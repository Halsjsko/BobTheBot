using System;
using System.Collections.Generic;
using Discord;
using Discord.Interactions;
using UnitsNet;
using UnitsNet.Units;

namespace Commands.Helpers
{
    public static class UnitConversion
    {
        /// <summary>
        /// Get the quantity type for the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit type to get the quantity type for.</param>
        public static Type GetQuantityType(UnitType unitType)
        {
            return unitType switch
            {
                UnitType.Length => typeof(Length),
                UnitType.Mass => typeof(Mass),
                UnitType.Volume => typeof(Volume),
                UnitType.Area => typeof(Area),
                UnitType.Time => typeof(Duration),
                UnitType.Temperature => typeof(Temperature),
                UnitType.Pressure => typeof(Pressure),
                UnitType.Speed => typeof(Speed),
                UnitType.Energy => typeof(Energy),
                UnitType.Storage => typeof(Information),
                UnitType.Angle => typeof(Angle),
                UnitType.Frequency => typeof(Frequency),
                _ => null
            };
        }

        /// <summary>
        /// Get the unit enum type for the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit type to get the unit enum type for.</param>
        public static Type GetUnitEnumType(UnitType unitType)
        {
            return unitType switch
            {
                UnitType.Length => typeof(LengthUnit),
                UnitType.Mass => typeof(MassUnit),
                UnitType.Volume => typeof(VolumeUnit),
                UnitType.Area => typeof(AreaUnit),
                UnitType.Time => typeof(DurationUnit),
                UnitType.Temperature => typeof(TemperatureUnit),
                UnitType.Pressure => typeof(PressureUnit),
                UnitType.Speed => typeof(SpeedUnit),
                UnitType.Energy => typeof(EnergyUnit),
                UnitType.Storage => typeof(InformationUnit),
                UnitType.Angle => typeof(AngleUnit),
                UnitType.Frequency => typeof(FrequencyUnit),
                _ => null
            };
        }

        /// <summary>
        /// Get a list of valid units for the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit type to get the valid units for.</param>
        public static string GetValidUnits(UnitType unitType)
        {
            return unitType switch
            {
                UnitType.Length => $"{string.Join(", ", Enum.GetNames(typeof(LengthUnit)))}.\n- Plural forms are accepted such as meters.\n- Abbreviated forms are accepted like m (these are case sensitive).",
                UnitType.Mass => $"{string.Join(", ", Enum.GetNames(typeof(MassUnit)))}.\n- Plural forms are accepted such as kilograms.\n- Abbreviated forms are accepted like kg (these are case sensitive).",
                UnitType.Volume => $"{string.Join(", ", Enum.GetNames(typeof(VolumeUnit)))}.\n- Plural forms are accepted such as liters.\n- Abbreviated forms are accepted like L (these are case sensitive).\n- Special characters are also accepted like m³",
                UnitType.Area => $"{string.Join(", ", Enum.GetNames(typeof(AreaUnit)))}.\n- Plural forms are accepted such as square meters.\n- Abbreviated forms are accepted like sq m (these are case sensitive).\n- Special characters are also accepted like m²",
                UnitType.Time => $"{string.Join(", ", Enum.GetNames(typeof(DurationUnit)))}.\n- Plural forms are accepted such as seconds.\n- Abbreviated forms are accepted like s (these are case sensitive).",
                UnitType.Temperature => $"{string.Join(", ", Enum.GetNames(typeof(TemperatureUnit)))}.\n- Plural forms are accepted such as degrees Celsius.\n- Abbreviated forms are accepted like C (these are case sensitive).\n- Special characters are also accepted like °C",
                UnitType.Pressure => $"{string.Join(", ", Enum.GetNames(typeof(PressureUnit)))}.\n- Plural forms are accepted such as pascals.\n- Abbreviated forms are accepted like Pa (these are case sensitive).",
                UnitType.Speed => $"{string.Join(", ", Enum.GetNames(typeof(SpeedUnit)))}.\n- Plural forms are accepted such as meters per second.\n- Abbreviated forms are accepted like m/s (these are case sensitive).",
                UnitType.Energy => $"{string.Join(", ", Enum.GetNames(typeof(EnergyUnit)))}.\n- Plural forms are accepted such as joules.\n- Abbreviated forms are accepted like J (these are case sensitive).",
                UnitType.Storage => $"{string.Join(", ", Enum.GetNames(typeof(InformationUnit)))}.\n- Plural forms are accepted such as bytes.\n- Abbreviated forms are accepted like B (these are case sensitive).",
                UnitType.Angle => $"{string.Join(", ", Enum.GetNames(typeof(AngleUnit)))}.\n- Plural forms are accepted such as degrees.\n- Abbreviated forms are accepted like deg (these are case sensitive).\n- Special characters are also accepted like °.",
                UnitType.Frequency => $"{string.Join(", ", Enum.GetNames(typeof(FrequencyUnit)))}.\n- Plural forms are accepted such as hertz.\n- Abbreviated forms are accepted like Hz (these are case sensitive).",
                _ => "No valid units available.",
            };
        }

        /// <summary>
        /// Try parsing a unit with enhanced logic to handle common aliases and variations.
        /// </summary>
        /// <param name="unit">The unit to parse.</param>
        /// <param name="unitEnumType">The unit enum type to parse the unit with.</param>
        /// <param name="unitEnum">The parsed unit enum value.</param>
        public static bool TryParseUnit(string unit, Type unitEnumType, out Enum unitEnum)
        {
            // Try parsing with original case-sensitive input
            if (UnitsNetSetup.Default.UnitParser.TryParse(unit, unitEnumType, out unitEnum))
                return true;

            // Normalize input: lowercase and remove spaces
            string normalizedUnit = unit.ToLower().Replace(" ", "");

            // Check if normalized unit matches an alias and retry parsing if found
            if (UnitAliases.TryGetValue(normalizedUnit, out string alias) && UnitsNetSetup.Default.UnitParser.TryParse(alias, unitEnumType, out unitEnum))
            {
                return true;
            }

            unitEnum = null;
            return false;
        }

        /// <summary>
        /// Get the emoji for the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit type to get the emoji for.</param>
        public static string GetUnitTypeEmoji(UnitType unitType)
        {
            return unitType switch
            {
                UnitType.Length => "📏",
                UnitType.Mass => "⚖️",
                UnitType.Volume => "🥤",
                UnitType.Area => "📐",
                UnitType.Time => "⏳",
                UnitType.Temperature => "🌡️",
                UnitType.Pressure => "🧊",
                UnitType.Speed => "🏎️",
                UnitType.Energy => "⚡",
                UnitType.Storage => "💾",
                UnitType.Angle => "🔄",
                UnitType.Frequency => "📡",
                _ => "❓"
            };
        }

        /// <summary>
        /// Get a suggestion button for the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit type to get the suggestion button for.</param>
        public static MessageComponent GetSuggestionButton(UnitType unitType)
        {
            return new ComponentBuilder()
                .WithButton(
                        label: "Suggest a Unit",
                        customId: $"suggestUnit:{unitType}",
                        style: ButtonStyle.Primary,
                        emote: Emoji.Parse(GetUnitTypeEmoji(unitType))
                    )
                    .Build();
        }

        /// <summary>
        /// The unit type for the conversion.
        /// </summary>
        public enum UnitType
        {
            [ChoiceDisplay("Length (Distance, Height, Width)")]
            Length,
            [ChoiceDisplay("Mass (Weight)")]
            Mass,
            [ChoiceDisplay("Volume (Capacity)")]
            Volume,
            [ChoiceDisplay("Area (Surface)")]
            Area,
            [ChoiceDisplay("Duration (Time)")]
            Time,
            Temperature,
            [ChoiceDisplay("Pressure (Stress)")]
            Pressure,
            [ChoiceDisplay("Speed (Velocity)")]
            Speed,
            [ChoiceDisplay("Energy (Work)")]
            Energy,
            [ChoiceDisplay("Information (Storage)")]
            Storage,
            Angle,
            [ChoiceDisplay("Frequency (Wavelength, Pitch, Tempo, Refresh Rate)")]
            Frequency
        }

        private static readonly Dictionary<string, string> UnitAliases = new()
        {
            // Length units
            { "angstrom", "Å" },
            { "angstroms", "Å" },
            { "astronomicalunit", "AU" },
            { "astronomicalunits", "AU" },
            { "centimeter", "cm" },
            { "centimeters", "cm" },
            { "chain", "ch" },
            { "chains", "ch" },
            { "datamile", "dm" },
            { "datamiles", "dm" },
            { "decameter", "dam" },
            { "decameters", "dam" },
            { "decimeter", "dm" },
            { "decimeters", "dm" },
            { "dtpica", "pc" },
            { "dtpicas", "pc" },
            { "dtppoint", "pt" },
            { "dtppoints", "pt" },
            { "fathom", "ftm" },
            { "fathoms", "ftm" },
            { "foot", "ft" },
            { "feet", "ft" },
            { "hand", "h" },
            { "hands", "h" },
            { "hectometer", "hm" },
            { "hectometers", "hm" },
            { "inch", "in" },
            { "inches", "in" },
            { "kilolightyear", "kly" },
            { "kilolightyears", "kly" },
            { "kilometer", "km" },
            { "kilometers", "km" },
            { "kiloparsec", "kpc" },
            { "kiloparsecs", "kpc" },
            { "lightyear", "ly" },
            { "lightyears", "ly" },
            { "megalightyear", "Mly" },
            { "megalightyears", "Mly" },
            { "megaparsec", "Mpc" },
            { "megaparsecs", "Mpc" },
            { "meter", "m" },
            { "meters", "m" },
            { "microinch", "μin" },
            { "microinches", "μin" },
            { "micrometer", "μm" },
            { "micrometers", "μm" },
            { "mil", "mil" },
            { "mils", "mil" },
            { "mile", "mi" },
            { "miles", "mi" },
            { "millimeter", "mm" },
            { "millimeters", "mm" },
            { "nanometer", "nm" },
            { "nanometers", "nm" },
            { "nauticalmile", "NM" },
            { "nauticalmiles", "NM" },
            { "parsec", "pc" },
            { "parsecs", "pc" },
            { "printerpica", "pc" },
            { "printerpicas", "pc" },
            { "printerpoint", "pt" },
            { "printerpoints", "pt" },
            { "shackle", "shackle" },
            { "shackles", "shackle" },
            { "solarradius", "R☉" },
            { "solarradii", "R☉" },
            { "twip", "twip" },
            { "twips", "twip" },
            { "ussurveyfoot", "ftUS" },
            { "ussurveyfeet", "ftUS" },
            { "yard", "yd" },
            { "yards", "yd" },
            { "kiloyard", "kyd" },
            { "kiloyards", "kyd" },
            { "megameter", "Mm" },
            { "megameters", "Mm" },
            { "picometer", "pm" },
            { "picometers", "pm" },
            { "gigameter", "Gm" },
            { "gigameters", "Gm" },
            { "kilofoot", "kft" },
            { "kilofoots", "kft" }, // Rarely used but included for completeness
            { "femtometer", "fm" },
            { "femtometers", "fm" },

            // Storage units
            { "bit", "b" },
            { "bits", "b" },
            { "byte", "B" },
            { "bytes", "B" },
            { "exabit", "Eb" },
            { "exabits", "Eb" },
            { "exabyte", "EB" },
            { "exabytes", "EB" },
            { "exbibit", "Eib" },
            { "exbibits", "Eib" },
            { "exbibyte", "EiB" },
            { "exbibytes", "EiB" },
            { "gibibit", "Gib" },
            { "gibibits", "Gib" },
            { "gibibyte", "GiB" },
            { "gibibytes", "GiB" },
            { "gigabit", "Gb" },
            { "gigabits", "Gb" },
            { "gigabyte", "GB" },
            { "gigabytes", "GB" },
            { "kibibit", "Kib" },
            { "kibibits", "Kib" },
            { "kibibyte", "KiB" },
            { "kibibytes", "KiB" },
            { "kilobit", "kb" },
            { "kilobits", "kb" },
            { "kilobyte", "kB" },
            { "kilobytes", "kB" },
            { "mebibit", "Mib" },
            { "mebibits", "Mib" },
            { "mebibyte", "MiB" },
            { "mebibytes", "MiB" },
            { "megabit", "Mb" },
            { "megabits", "Mb" },
            { "megabyte", "MB" },
            { "megabytes", "MB" },
            { "pebibit", "Pib" },
            { "pebibits", "Pib" },
            { "pebibyte", "PiB" },
            { "pebibytes", "PiB" },
            { "petabit", "Pb" },
            { "petabits", "Pb" },
            { "petabyte", "PB" },
            { "petabytes", "PB" },
            { "tebibit", "Tib" },
            { "tebibits", "Tib" },
            { "tebibyte", "TiB" },
            { "tebibytes", "TiB" },
            { "terabit", "Tb" },
            { "terabits", "Tb" },
            { "terabyte", "TB" },
            { "terabytes", "TB" },

            // Angular units
            { "arcminute", "′" },
            { "arcminutes", "′" },
            { "arcsecond", "″" },
            { "arcseconds", "″" },
            { "centiradian", "crad" },
            { "centiradians", "crad" },
            { "deciradian", "drad" },
            { "deciradians", "drad" },
            { "degree", "°" },
            { "degrees", "°" },
            { "gradian", "gon" },
            { "gradians", "gon" },
            { "microdegree", "μ°" },
            { "microdegrees", "μ°" },
            { "microradian", "μrad" },
            { "microradians", "μrad" },
            { "millidegree", "m°" },
            { "millidegrees", "m°" },
            { "milliradian", "mrad" },
            { "milliradians", "mrad" },
            { "nanodegree", "n°" },
            { "nanodegrees", "n°" },
            { "nanoradian", "nrad" },
            { "nanoradians", "nrad" },
            { "natomil", "mil" },
            { "natomils", "mil" },
            { "radian", "rad" },
            { "radians", "rad" },
            { "revolution", "rev" },
            { "revolutions", "rev" },
            { "tilt", "tilt" },
            { "tilts", "tilt" },

            // Mass units
            { "centigram", "cg" },
            { "centigrams", "cg" },
            { "decagram", "dag" },
            { "decagrams", "dag" },
            { "decigram", "dg" },
            { "decigrams", "dg" },
            { "earthmass", "M⊕" },
            { "earthmasses", "M⊕" },
            { "grain", "gr" },
            { "grains", "gr" },
            { "gram", "g" },
            { "grams", "g" },
            { "hectogram", "hg" },
            { "hectograms", "hg" },
            { "kilogram", "kg" },
            { "kilograms", "kg" },
            { "kilopound", "klb" },
            { "kilopounds", "klb" },
            { "kilotonne", "kt" },
            { "kilotonnes", "kt" },
            { "kiloton", "kt" },
            { "kilotons", "kt" },
            { "longhundredweight", "lcwt" },
            { "longhundredweights", "lcwt" },
            { "longton", "LT" },
            { "longtons", "LT" },
            { "megapound", "Mlb" },
            { "megapounds", "Mlb" },
            { "megatonne", "Mt" },
            { "megatonnes", "Mt" },
            { "megaton", "Mt" },
            { "megatons", "Mt" },
            { "microgram", "μg" },
            { "micrograms", "μg" },
            { "milligram", "mg" },
            { "milligrams", "mg" },
            { "nanogram", "ng" },
            { "nanograms", "ng" },
            { "ounce", "oz" },
            { "ounces", "oz" },
            { "pound", "lb" },
            { "pounds", "lb" },
            { "shorthundredweight", "shcwt" },
            { "shorthundredweights", "shcwt" },
            { "shortton", "st" },
            { "shorttons", "st" },
            { "slug", "slug" },
            { "slugs", "slug" },
            { "solarmass", "M☉" },
            { "solarmasses", "M☉" },
            { "stone", "st" },
            { "stones", "st" },
            { "tonne", "t" },
            { "tonnes", "t" },
            { "ton", "t" },
            { "tons", "t" },
            { "picogram", "pg" },
            { "picograms", "pg" },
            { "femtogram", "fg" },
            { "femtograms", "fg" },

            // Energy units
            { "britishthermalunit", "BTU" },
            { "britishthermalunits", "BTUs" },
            { "calorie", "cal" },
            { "calories", "cals" },
            { "decathermeec", "dth(e)" },
            { "decathermeecs", "dth(e)s" },
            { "decathermimperial", "dth(i)" },
            { "decathermimperials", "dth(i)s" },
            { "decathermus", "dth" },
            { "decathermuses", "dths" },
            { "electronvolt", "eV" },
            { "electronvolts", "eVs" },
            { "erg", "erg" },
            { "ergs", "ergs" },
            { "footpound", "ft·lbf" },
            { "footpounds", "ft·lbf" },
            { "gigabritishthermalunit", "GBTU" },
            { "gigabritishthermalunits", "GBTUs" },
            { "gigaelectronvolt", "GeV" },
            { "gigaelectronvolts", "GeVs" },
            { "gigajoule", "GJ" },
            { "gigajoules", "GJs" },
            { "gigawattday", "GWd" },
            { "gigawattdays", "GWd" },
            { "gigawatthour", "GWh" },
            { "gigawatthours", "GWh" },
            { "horsepowerhour", "hp·h" },
            { "horsepowerhours", "hp·h" },
            { "joule", "J" },
            { "joules", "J" },
            { "kilobritishthermalunit", "kBTU" },
            { "kilobritishthermalunits", "kBTUs" },
            { "kilocalorie", "kcal" },
            { "kilocalories", "kcals" },
            { "kiloelectronvolt", "keV" },
            { "kiloelectronvolts", "keVs" },
            { "kilojoule", "kJ" },
            { "kilojoules", "kJs" },
            { "kilowattday", "kWd" },
            { "kilowattdays", "kWd" },
            { "kilowatthour", "kWh" },
            { "kilowatthours", "kWh" },
            { "megabritishthermalunit", "MBTU" },
            { "megabritishthermalunits", "MBTUs" },
            { "megacalorie", "Mcal" },
            { "megacalories", "Mcals" },
            { "megaelectronvolt", "MeV" },
            { "megaelectronvolts", "MeVs" },
            { "megajoule", "MJ" },
            { "megajoules", "MJs" },
            { "megawattday", "MWd" },
            { "megawattdays", "MWd" },
            { "megawatthour", "MWh" },
            { "megawatthours", "MWh" },
            { "millijoule", "mJ" },
            { "millijoules", "mJs" },
            { "teraelectronvolt", "TeV" },
            { "teraelectronvolts", "TeVs" },
            { "terawattday", "TWd" },
            { "terawattdays", "TWd" },
            { "terawatthour", "TWh" },
            { "terawatthours", "TWh" },
            { "thermeec", "therm(e)" },
            { "thermeecs", "therm(e)s" },
            { "thermimperial", "therm(i)" },
            { "thermimperials", "therm(i)s" },
            { "thermus", "therm" },
            { "thermi", "therms" },
            { "wattday", "Wd" },
            { "wattdays", "Wd" },
            { "watthour", "Wh" },
            { "watthours", "Wh" },
            { "terajoule", "TJ" },
            { "terajoules", "TJs" },
            { "petajoule", "PJ" },
            { "petajoules", "PJs" },
            { "nanojoule", "nJ" },
            { "nanojoules", "nJs" },
            { "microjoule", "μJ" },
            { "microjoules", "μJs" },

            // Velocity units
            { "centimeterperhour", "cm/h" },
            { "centimeterperhours", "cm/h" },
            { "centimeterperminute", "cm/min" },
            { "centimeterperminutes", "cm/min" },
            { "centimeterpersecond", "cm/s" },
            { "centimeterperseconds", "cm/s" },
            { "decimeterperminute", "dm/min" },
            { "decimeterperminutes", "dm/min" },
            { "decimeterpersecond", "dm/s" },
            { "decimeterperseconds", "dm/s" },
            { "footperhour", "ft/h" },
            { "footperhours", "ft/h" },
            { "footperminute", "ft/min" },
            { "footperminutes", "ft/min" },
            { "footpersecond", "ft/s" },
            { "footperseconds", "ft/s" },
            { "inchperhour", "in/h" },
            { "inchperhours", "in/h" },
            { "inchperminute", "in/min" },
            { "inchperminutes", "in/min" },
            { "inchpersecond", "in/s" },
            { "inchperseconds", "in/s" },
            { "kilometerperhour", "km/h" },
            { "kilometerperhours", "km/h" },
            { "kilometerperminute", "km/min" },
            { "kilometerperminutes", "km/min" },
            { "kilometerpersecond", "km/s" },
            { "kilometerperseconds", "km/s" },
            { "knot", "kn" },
            { "knots", "kn" },
            { "meterperhour", "m/h" },
            { "meterperhours", "m/h" },
            { "meterperminute", "m/min" },
            { "meterperminutes", "m/min" },
            { "meterpersecond", "m/s" },
            { "meterperseconds", "m/s" },
            { "micrometerperminute", "μm/min" },
            { "micrometerperminutes", "μm/min" },
            { "micrometerpersecond", "μm/s" },
            { "micrometerperseconds", "μm/s" },
            { "mileperhour", "mph" },
            { "mileperhours", "mph" },
            { "millimeterperhour", "mm/h" },
            { "millimeterperhours", "mm/h" },
            { "millimeterperminute", "mm/min" },
            { "millimeterperminutes", "mm/min" },
            { "millimeterpersecond", "mm/s" },
            { "millimeterperseconds", "mm/s" },
            { "nanometerperminute", "nm/min" },
            { "nanometerperminutes", "nm/min" },
            { "nanometerpersecond", "nm/s" },
            { "nanometerperseconds", "nm/s" },
            { "ussurveyfootperhour", "ftUS/h" },
            { "ussurveyfootperhours", "ftUS/h" },
            { "ussurveyfootperminute", "ftUS/min" },
            { "ussurveyfootperminutes", "ftUS/min" },
            { "ussurveyfootpersecond", "ftUS/s" },
            { "ussurveyfootperseconds", "ftUS/s" },
            { "yardperhour", "yd/h" },
            { "yardperhours", "yd/h" },
            { "yardperminute", "yd/min" },
            { "yardperminutes", "yd/min" },
            { "yardpersecond", "yd/s" },
            { "yardperseconds", "yd/s" },
            { "mach", "Ma" },
            { "machs", "Ma" },

            // Pressure units
            { "atmosphere", "atm" },
            { "atmospheres", "atm" },
            { "bar", "bar" },
            { "bars", "bar" },
            { "centibar", "cbar" },
            { "centibars", "cbar" },
            { "decapascal", "dPa" },
            { "decapascals", "dPa" },
            { "decibar", "dbar" },
            { "decibars", "dbar" },
            { "dynepersquarecentimeter", "dyn/cm²" },
            { "dynepersquarecentimeters", "dyn/cm²" },
            { "footofelevation", "ft.elev" },
            { "footofelevations", "ft.elev" },
            { "footofhead", "ft.head" },
            { "footofheads", "ft.head" },
            { "gigapascal", "GPa" },
            { "gigapascals", "GPa" },
            { "hectopascal", "hPa" },
            { "hectopascals", "hPa" },
            { "inchofmercury", "inHg" },
            { "inchofmercuries", "inHg" },
            { "inchofwatercolumn", "inH₂O" },
            { "inchofwatercolumns", "inH₂O" },
            { "kilobar", "kbar" },
            { "kilobars", "kbar" },
            { "kilogramforcepersquarecentimeter", "kgf/cm²" },
            { "kilogramforcepersquarecentimeters", "kgf/cm²" },
            { "kilopascal", "kPa" },
            { "kilopascals", "kPa" },
            { "megapascal", "MPa" },
            { "megapascals", "MPa" },
            { "milliinchofmercury", "milli-inHg" },
            { "milliinchofmercuries", "milli-inHg" },
            { "milliinchofwatercolumn", "milli-inH₂O" },
            { "milliinchofwatercolumns", "milli-inH₂O" },
            { "newtonspersquaremeter", "N/m²" },
            { "newtonspersquaremeters", "N/m²" },
            { "pascal", "Pa" },
            { "pascals", "Pa" },
            { "poundperfoot", "lb/ft" },
            { "poundperfeet", "lb/ft" },
            { "poundpersquarefoot", "lb/ft²" },
            { "poundpersquarefeet", "lb/ft²" },
            { "poundpersquareinch", "lb/in²" },
            { "poundpersquareinches", "lb/in²" },
            { "psia", "psia" },
            { "psias", "psia" },
            { "psig", "psig" },
            { "psigs", "psig" },
            { "torr", "torr" },
            { "torrs", "torr" },

            // Area units
            { "acre", "acre" },
            { "acres", "acres" },
            { "are", "are" },
            { "ares", "ares" },
            { "centiare", "c㎡" },
            { "centiares", "c㎡" },
            { "hectare", "ha" },
            { "hectares", "ha" },
            { "squarecentimeter", "cm²" },
            { "squarecentimeters", "cm²" },
            { "squaredecimeter", "dm²" },
            { "squaredecimeters", "dm²" },
            { "squarefoot", "ft²" },
            { "squarefeet", "ft²" },
            { "squareinch", "in²" },
            { "squareinches", "in²" },
            { "squarekilometer", "km²" },
            { "squarekilometers", "km²" },
            { "squaremeter", "m²" },
            { "squaremeters", "m²" },
            { "squaremillimeter", "mm²" },
            { "squaremillimeters", "mm²" },
            { "squaremile", "mi²" },
            { "squaremiles", "mi²" },
            { "squareyard", "yd²" },
            { "squareyards", "yd²" },

            // Additional square cases
            { "sqmeter", "m²" },
            { "sqm", "m²" },
            { "sqmeters", "m²" },
            { "sqcentimeter", "cm²" },
            { "sqcentimeters", "cm²" },
            { "sqdecimeter", "dm²" },
            { "sqdecimeters", "dm²" },
            { "sqfoot", "ft²" },
            { "sqfeet", "ft²" },
            { "sqft", "ft²" },
            { "sqinch", "in²" },
            { "sqin", "in²" },
            { "sqinches", "in²" },
            { "sqkilometer", "km²" },
            { "sqkm", "km²" },
            { "sqkilometers", "km²" },
            { "sqmillimeter", "mm²" },
            { "sqmm", "mm²" },
            { "sqmillimeters", "mm²" },
            { "sqyard", "yd²" },
            { "sqyd", "yd²" },
            { "sqyards", "yd²" },
            { "sqmile", "mi²" },
            { "sqmi", "mi²" },
            { "sqmiles", "mi²" },

            // Volume units
            { "acrefoot", "acre·ft" },
            { "acrefeet", "acre·ft" },
            { "centiliter", "cL" },
            { "centiliters", "cL" },
            { "cubiccentimeter", "cm³" },
            { "cubiccentimeters", "cm³" },
            { "cubicdecimeter", "dm³" },
            { "cubicdecimeters", "dm³" },
            { "cubicfoot", "ft³" },
            { "cubicfeet", "ft³" },
            { "cubicinch", "in³" },
            { "cubicinches", "in³" },
            { "cubickilometer", "km³" },
            { "cubickilometers", "km³" },
            { "cubicmeter", "m³" },
            { "cubicmeters", "m³" },
            { "cubicmillimeter", "mm³" },
            { "cubicmillimeters", "mm³" },
            { "cubicyard", "yd³" },
            { "cubicyards", "yd³" },
            { "dekaliter", "daL" },
            { "dekaliters", "daL" },
            { "deciliter", "dL" },
            { "deciliters", "dL" },
            { "gallon", "gal" },
            { "gallons", "gal" },
            { "hectoliter", "hL" },
            { "hectoliters", "hL" },
            { "liter", "L" },
            { "liters", "L" },
            { "milliliter", "mL" },
            { "milliliters", "mL" },
            { "pint", "pt" },
            { "pints", "pt" },
            { "quart", "qt" },
            { "quarts", "qt" },
            { "tablespoon", "tbsp" },
            { "tablespoons", "tbsp" },
            { "teaspoon", "tsp" },
            { "teaspoons", "tsp" },

            // Additional cubic cases
            { "metercubed", "m³" },
            { "meterscubed", "m³" },
            { "centimetercubed", "cm³" },
            { "centimeterscubed", "cm³" },
            { "decimetercubed", "dm³" },
            { "decimeterscubed", "dm³" },
            { "inchcubed", "in³" },
            { "inchescubed", "in³" },
            { "footcubed", "ft³" },
            { "feetcubed", "ft³" },
            { "yardcubed", "yd³" },
            { "yardscubed", "yd³" },
            { "kilometercubed", "km³" },
            { "kilometerscubed", "km³" },
            { "millimetercubed", "mm³" },
            { "millimeterscubed", "mm³" },

            // Temperature units
            { "celsius", "°C" },
            { "c", "°C" },
            { "°c", "°C" },
            { "celsiuses", "°C" },
            { "fahrenheit", "°F" },
            { "f", "°F" },
            { "°f", "°F" },
            { "fahrenheits", "°F" },
            { "kelvin", "K" },
            { "k", "K" },
            { "kelvins", "K" },

            // Time units
            { "femtosecond", "fs" },
            { "femtoseconds", "fs" },
            { "gigasecond", "Gs" },
            { "gigaseconds", "Gs" },
            { "hour", "h" },
            { "hours", "h" },
            { "microsecond", "μs" },
            { "microseconds", "μs" },
            { "millisecond", "ms" },
            { "milliseconds", "ms" },
            { "nanosecond", "ns" },
            { "nanoseconds", "ns" },
            { "second", "s" },
            { "seconds", "s" },
            { "terasecond", "Ts" },
            { "teraseconds", "Ts" },
            { "year", "y" },
            { "years", "y" },

            // Frequency and rate units
            { "beatperminute", "bpm" },
            { "beatsperminute", "bpm" },
            { "beatpermin", "bpm" },
            { "beatspermin", "bpm" },
            { "bunit", "BUnit" },
            { "bunits", "BUnit" },
            { "cycleperhour", "cph" },
            { "cyclesperhour", "cph" },
            { "cycleperhr", "cph" },
            { "cyclesperhr", "cph" },
            { "cycleperminute", "cpm" },
            { "cyclesperminute", "cpm" },
            { "cyclepermin", "cpm" },
            { "cyclespermin", "cpm" },
            { "gigahertz", "GHz" },
            { "gigahertzes", "GHz" },
            { "ghz", "GHz" },
            { "hertz", "Hz" },
            { "hertzes", "Hz" },
            { "hz", "Hz" },
            { "kilohertz", "kHz" },
            { "kilohertzes", "kHz" },
            { "khz", "kHz" },
            { "megahertz", "MHz" },
            { "megahertzes", "MHz" },
            { "persecond", "1/s" },
            { "persec", "1/s" },
            { "radianpersecond", "rad/s" },
            { "radianspersecond", "rad/s" },
            { "radianpersec", "rad/s" },
            { "radianspersec", "rad/s" },
            { "terahertz", "THz" },
            { "terahertzes", "THz" },
            { "thz", "THz" },
            { "microhertz", "µHz" },
            { "microhertzes", "µHz" },
            { "mhz", "µHz" },
            { "millihertz", "mHz" },
            { "millihertzes", "mHz" },
        };
    }
}