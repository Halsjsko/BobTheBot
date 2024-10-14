using System;
using System.Collections.Generic;
using static ApiInteractions.Interface;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ColorHelper;
using Discord.Interactions;
using static Commands.Helpers.Choose;
using System.IO;
using Commands.Helpers;
using Discord;
using System.Net.Http;

namespace Commands
{
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [Group("random", "All random (RNG) commands.")]
    public class RandomGroup : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Random random = new();

        [SlashCommand("dice-roll", "Bob will roll a die with the side amount specified.")]
        public async Task DiceRoll(int sides)
        {
            if (sides <= 0)
            {
                await RespondAsync(text: $"🌌 The die formed a *rift* in our dimension! **Maybe** try using a number **greater than 0**", ephemeral: true);
            }
            else
            {
                int randInt = random.Next(1, sides + 1);
                await RespondAsync(text: $"🎲 The {sides} sided die landed on **{randInt}**");
            }
        }

        [SlashCommand("date", "Bob will will pick a random date.")]
        public async Task RandomDate([Summary("earliestYear", "A year that is as early as you want the date to occur in.")] int earliestYear, [Summary("latestYear", "A year that is as late as you want the date to occur in.")] int latestYear)
        {
            if (latestYear < 0 || earliestYear < 0)
            {
                await RespondAsync(text: $"🌌 *Whoa!* a *rift* in our dimension! **Maybe** try a year that is **atleast 0**.");
            }
            else if (latestYear.ToString().Length > 8 || earliestYear.ToString().Length > 8)
            {
                await RespondAsync(text: $"❌ Please, choose a year that is **8 digits or less**.");
            }
            else if (earliestYear > latestYear)
            {
                await RespondAsync(text: $"❌ Please, make the *earliest year* **smaller** than the *latest year*.");
            }
            else
            {
                // Pick Year
                int year = random.Next(earliestYear, latestYear + 1);

                // Pick Month
                (string name, int days)[] months = { ("January", 31), ("February", 28), ("March", 31), ("April", 30), ("May", 31), ("June", 30), ("July", 31), ("August", 31), ("September", 30), ("October", 31), ("November", 30), ("December", 31) };
                (string name, int days) = months[random.Next(0, months.Length)];

                // Pick Day
                int day = random.Next(1, days + 1);

                await RespondAsync(text: $":calendar_spiral: {name} {day}, {year}");
            }
        }

        [SlashCommand("8ball", "Bob will shake a magic 8 ball in response to a question.")]
        public async Task Magic8Ball(string prompt)
        {
            // Possible Answers
            string[] results = { "'no'", "'yes'", "'maybe'", "'ask again'", "'probably not'", "'affirmative'", "'it is certain'", "'very doubtful'", "'regretfully.. yes'", "'try again later...'" };

            // Respond
            string formattedText = $"🎱 **{results[random.Next(0, results.Length)]}** in response to {prompt}";
            if (formattedText.Length > 2000)
            {
                await RespondAsync($"❌ The magic 8ball broke because your prompt had **{formattedText.Length}** characters.\n- Try having fewer characters.\n- Discord has a limit of **2000** characters.", ephemeral: true);
            }
            else
            {
                await RespondAsync(text: formattedText);
            }
        }

        [SlashCommand("choose", "Can't make up your mind? Bob can for you!")]
        public async Task Pick(string option1, string option2, string option3 = "", string option4 = "", string option5 = "")
        {
            List<string> choices = new()
        {
            option1,
            option2
        };
            AddChoice(option3, choices);
            AddChoice(option4, choices);
            AddChoice(option5, choices);

            foreach (string s in choices)
            {
                if (s.Length > 1024) // 1024 is arbitrary
                {
                    choices.Remove(s);
                }
            }

            if (choices.Count <= 0)
            {
                await RespondAsync($"❌ Bob *cannot* decide because your choices contain to many characters.\n- Try having fewer characters.", ephemeral: true);
            }
            else
            {
                string choice = choices[random.Next(0, choices.Count)];
                await RespondAsync(text: "🤔 " + GetRandomDecisionText() + $"**{choice}**");
            }
        }

        [SlashCommand("color", "Bob will choose a random color.")]
        public async Task Color()
        {
            // Get Values
            var hex = string.Format("{0:X6}", random.Next(0x1000000));
            CMYK cmyk = ColorConverter.HexToCmyk(new HEX(hex));
            HSL hsl = ColorConverter.HexToHsl(new HEX(hex));
            HSV hsv = ColorConverter.HexToHsv(new HEX(hex));
            RGB rgb = ColorConverter.HexToRgb(new HEX(hex));
            Color displayColor = new(Convert.ToUInt32(hex, 16));

            // Make Color Image
            using MemoryStream imageStream = ColorPreview.CreateColorImage(100, 100, hex);

            // Make Embed
            var embed = new EmbedBuilder { };
            embed.AddField("Hex", $"```#{hex}```")
            .AddField("RGB", $"```R: {rgb.R}, G: {rgb.G}, B: {rgb.B}```")
            .AddField("CMYK", $"```C: {cmyk.C}, M: {cmyk.M}, Y: {cmyk.Y}, K: {cmyk.K}```")
            .AddField("HSL", $"```H: {hsl.H}, S: {hsl.S}, L: {hsl.L}```")
            .AddField("HSV", $"```H: {hsv.H}, S: {hsv.S}, V: {hsv.V}```")
            .WithThumbnailUrl("attachment://image.png")
            .WithColor(displayColor);

            await RespondWithFileAsync(imageStream, "image.png", "", embed: embed.Build());
        }

        [CommandContextType(InteractionContextType.Guild | InteractionContextType.PrivateChannel)]
        [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
        [SlashCommand("quote", "Bob will tell you a quote.")]
        public async Task Quote([Summary("prompt", "use /quote-prompts to see all valid prompts")] string prompt = "")
        {
            string content;

            try
            {
                content = await GetFromAPI($"http://api.quotable.io/quotes/random?tags={prompt}", AcceptTypes.application_json);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while fetching the quote: {ex.Message}");
                await RespondAsync(text: "❌ There was an issue getting a quote from the API (quotable.io).\n- This is out of Bob's control unfortunately.\n- Please try again later.", ephemeral: true);
                return;
            }

            // Check if the content is empty or indicates no quotes found
            if (content != "[]")
            {
                // Parse Content
                var formattedContent = content[1..^1];
                var jsonData = JsonNode.Parse(formattedContent).AsObject();
                var quote = jsonData["content"].ToString();
                var author = jsonData["author"].ToString();

                // Respond with the quote
                await RespondAsync(text: $"✍️ {quote} - *{author}*");
            }
            else
            {
                // Respond if no quotes match the prompt
                await RespondAsync(text: $"❌ The prompt: {prompt} was not recognized. Use `/quote-prompts` to see all valid prompts.\n- If you think this is a mistake, let us know here: [Bob's Official Server](https://discord.gg/HvGMRZD8jQ)", ephemeral: true);
            }
        }

        [SlashCommand("coin-toss", "Bob will flip a coin")]
        public async Task CoinToss()
        {
            int randInt = random.Next(0, 2);
            string result;
            if (randInt == 1)
            {
                result = "heads";
            }
            else
            {
                result = "tails";
            }
            await RespondAsync(text: $"🪙 The coin landed **" + result + "**!");
        }

        [CommandContextType(InteractionContextType.Guild | InteractionContextType.PrivateChannel)]
        [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
        [SlashCommand("fact", "Bob will provide you with an outrageous fact.")]
        public async Task RandomFacts()
        {
            string content = await GetFromAPI("https://uselessfacts.jsph.pl/api/v2/facts/random?language=en", AcceptTypes.application_json);

            // Parse Content
            var jsonData = JsonNode.Parse(content).AsObject();
            var fact = jsonData["text"].ToString();

            StringBuilder factFormatted = new();

            for (int i = 0; i < fact.Length; i++)
            {
                if (fact[i] == '`')
                {
                    factFormatted.Append('\\');
                }
                factFormatted.Append(fact[i]);
            }

            await RespondAsync(text: $"🤓 {factFormatted}");
        }

        [CommandContextType(InteractionContextType.Guild | InteractionContextType.PrivateChannel)]
        [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
        [SlashCommand("dog", "Bob will find you a cute doggo image!")]
        public async Task RandomDog()
        {
            string content = await GetFromAPI("https://random.dog/woof.json", AcceptTypes.application_json);

            // Parse Content
            var jsonData = JsonNode.Parse(content).AsObject();
            var image = jsonData["url"].ToString();

            string[] dogEmojis = { "🐕", "🐶", "🐕‍🦺", "🐩" };

            await RespondAsync(text: $"{dogEmojis[random.Next(0, dogEmojis.Length)]}[dog]({image})");
        }

        [CommandContextType(InteractionContextType.Guild | InteractionContextType.PrivateChannel)]
        [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
        [SlashCommand("advice", "Bob will provide you with random advice.")]
        public async Task RandomAdvice()
        {
            string content = await GetFromAPI("https://api.adviceslip.com/advice", AcceptTypes.application_json);

            // Parse Content
            var jsonData = JsonNode.Parse(content).AsObject();
            var slip = JsonNode.Parse(jsonData["slip"].ToString()).AsObject();
            var advice = slip["advice"].ToString();

            // Respond
            await RespondAsync(text: $"🦉 *{advice}*");
        }

        [CommandContextType(InteractionContextType.Guild | InteractionContextType.PrivateChannel)]
        [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
        [SlashCommand("dad-joke", "Bob will tell you a dad joke.")]
        public async Task DadJoke()
        {
            string content = await GetFromAPI("https://icanhazdadjoke.com", AcceptTypes.text_plain);

            // Respond
            await RespondAsync(text: $"😉  *{content}*");
        }
    }
}