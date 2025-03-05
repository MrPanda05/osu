// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModDifficultyScalar : Mod, IApplicableToDifficulty
    {
        public override string Name => @"Difficulty Scalar";

        public override LocalisableString Description => @"Challenge yourself...";

        public override string Acronym => "DS";

        public override ModType Type => ModType.Conversion;

        public override IconUsage? Icon => FontAwesome.Solid.Calculator;

        public override double ScoreMultiplier => 0.5;

        public override bool RequiresConfiguration => true;

        public override Type[] IncompatibleMods => new[] { typeof(ModEasy), typeof(ModHardRock), typeof(ModDifficultyAdjust) };


        [SettingSource("HP multiplier", "Multiplies the HP value", 0, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        public DifficultyBindable DrainRateMultiplier { get; } = new DifficultyBindable
        {
            Precision = 0.01f,
            MinValue = 0,
            MaxValue = 2.5f,
            ExtendedMaxValue = 11,
            ReadCurrentFromDifficulty = diff => 1,
        };

        [SettingSource("Accuracy multiplier", "Multiplier the ACC/OD value", 1, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        public DifficultyBindable OverallDifficultyMultiplier { get; } = new DifficultyBindable
        {
            Precision = 0.01f,
            MinValue = 0,
            MaxValue = 2.5f,
            ExtendedMaxValue = 11,
            ReadCurrentFromDifficulty = diff => 1,
        };

        [SettingSource("HP Cap", "Caps the drain rate", 4, SettingControlType = typeof(MultiplierSettingsSlider))]
        public BindableDouble DrainCap { get; } = new BindableDouble(10)
        {
            Precision = 0.01,
            MinValue = 0,
            MaxValue = 11,
        };

        [SettingSource("Acc Cap", "Caps the overall difficulty", 5, SettingControlType = typeof(MultiplierSettingsSlider))]
        public BindableDouble OverallDifficultyCap { get; } = new BindableDouble(10)
        {
            Precision = 0.01,
            MinValue = 0,
            MaxValue = 11,
        };

        [SettingSource("Extended maximum", "Adjust multiplier limits for better fine tunning.")]
        public BindableBool ExtendedLimits { get; } = new BindableBool();

        protected ModDifficultyScalar()
        {
            foreach (var (_, property) in this.GetOrderedSettingsSourceProperties())
            {
                if (property.GetValue(this) is DifficultyBindable diffAdjustBindable)
                    diffAdjustBindable.ExtendedLimits.BindTo(ExtendedLimits);
            }
        }

        public override string SettingDescription
        {
            get
            {
                string drainRate = DrainRateMultiplier.IsDefault ? string.Empty : $"HP {DrainRateMultiplier.Value:N1}";
                string overallDifficulty = OverallDifficultyMultiplier.IsDefault ? string.Empty : $"OD {OverallDifficultyMultiplier.Value:N1}";

                return string.Join(", ", new[]
                {
                    drainRate,
                    overallDifficulty
                }.Where(s => !string.IsNullOrEmpty(s)));
            }
        }
        public void ReadFromDifficulty(IBeatmapDifficultyInfo difficulty)
        {
        }

        public void ApplyToDifficulty(BeatmapDifficulty difficulty) => ApplySettings(difficulty);

        /// <summary>
        /// Apply all custom settings to the provided beatmap.
        /// </summary>
        /// <param name="difficulty">The beatmap to have settings applied.</param>
        protected virtual void ApplySettings(BeatmapDifficulty difficulty)
        {
            //todo add cap so it does not goes out of range
            if (DrainRateMultiplier.Value != null)
            {
                float hpMultiplier = (float)(difficulty.DrainRate == 0 ? 0.01 * DrainRateMultiplier.Value.Value : difficulty.DrainRate * DrainRateMultiplier.Value.Value);
                difficulty.DrainRate = Math.Min(hpMultiplier, (float)DrainCap.Value);
            }
            if (OverallDifficultyMultiplier.Value != null)
            {
                float accMultiplier = (float)(difficulty.OverallDifficulty == 0 ? 0.01 * OverallDifficultyMultiplier.Value.Value : difficulty.OverallDifficulty * OverallDifficultyMultiplier.Value.Value);
                difficulty.OverallDifficulty = Math.Min(accMultiplier, (float)OverallDifficultyCap.Value);
            }
        }


    }
}
