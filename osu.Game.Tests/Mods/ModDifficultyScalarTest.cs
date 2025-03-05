// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Tests.Mods
{
    [TestFixture]
    public class ModDifficultyScalarTest
    {
        private TestOsuModDifficultyScalar testMod = null!;

        [SetUp]
        public void Setup()
        {
            testMod = new TestOsuModDifficultyScalar();
        }

        [Test]
        public void TestUnchangedSettingsFollowAppliedDifficulty()
        {

            var result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 10,
                OverallDifficulty = 10,
                ApproachRate = 10,
                CircleSize = 10
            });


            Assert.That(result.DrainRate, Is.EqualTo(10));
            Assert.That(result.OverallDifficulty, Is.EqualTo(10));
            Assert.That(result.ApproachRate, Is.EqualTo(10));
            Assert.That(result.CircleSize, Is.EqualTo(10));

            result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 3,
                OverallDifficulty = 7,
                ApproachRate = 4.5F,
                CircleSize = 5
            });


            Assert.That(result.DrainRate, Is.EqualTo(3));
            Assert.That(result.OverallDifficulty, Is.EqualTo(7));
            Assert.That(result.ApproachRate, Is.EqualTo(4.5f));
            Assert.That(result.CircleSize, Is.EqualTo(5));


        }
        [Test]
        public void TestChangedSettingsOverrideAppliedDifficultyDefaultCap()
        {
            //HR settings
            testMod.DrainRateMultiplier.Value = 1.4f;
            testMod.OverallDifficultyMultiplier.Value = 1.4f;
            testMod.ApproachRateMultiplier.Value = 1.4f;
            testMod.CircleSizeMultiplier.Value = 1.3f;


            var result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 10,
                OverallDifficulty = 10,
                ApproachRate = 10,
                CircleSize = 10
            });
            Assert.Multiple(() =>
            {
                Assert.That(result.DrainRate, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.OverallDifficulty, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.ApproachRate, Is.GreaterThanOrEqualTo(-10).And.LessThanOrEqualTo(10));
                Assert.That(result.CircleSize, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.DrainRate, Is.EqualTo(10));
                Assert.That(result.OverallDifficulty, Is.EqualTo(10));
                Assert.That(result.ApproachRate, Is.EqualTo(10));
                Assert.That(result.CircleSize, Is.EqualTo(10));
            });


            result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 1,
                OverallDifficulty = 1,
                ApproachRate = 1,
                CircleSize = 1
            });

            Assert.Multiple(() =>
            {
                Assert.That(result.DrainRate, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.OverallDifficulty, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.ApproachRate, Is.GreaterThanOrEqualTo(-10).And.LessThanOrEqualTo(10));
                Assert.That(result.CircleSize, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.DrainRate, Is.EqualTo(1.4f));
                Assert.That(result.OverallDifficulty, Is.EqualTo(1.4f));
                Assert.That(result.ApproachRate, Is.EqualTo(1.4f));
                Assert.That(result.CircleSize, Is.EqualTo(1.3f));
            });

            testMod.DrainRateMultiplier.Value = 2f;
            testMod.OverallDifficultyMultiplier.Value = 1.4f;
            testMod.ApproachRateMultiplier.Value = 0.4f;
            testMod.CircleSizeMultiplier.Value = 1f;

            result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 5.5f,
                OverallDifficulty = 4.6f,
                ApproachRate = 8,
                CircleSize = 4.3f
            });

            Assert.Multiple(() =>
            {
                Assert.That(result.DrainRate, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.OverallDifficulty, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.ApproachRate, Is.GreaterThanOrEqualTo(-10).And.LessThanOrEqualTo(10));
                Assert.That(result.CircleSize, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.DrainRate, Is.EqualTo(10));
                Assert.That(result.OverallDifficulty, Is.EqualTo(1.4f * 4.6f));
                Assert.That(result.ApproachRate, Is.EqualTo(3.2f));
                Assert.That(result.CircleSize, Is.EqualTo(4.3f));

            });


            //EZ settings
            testMod.DrainRateMultiplier.Value = 0.5f;
            testMod.OverallDifficultyMultiplier.Value = 0.5f;
            testMod.ApproachRateMultiplier.Value = 0.5f;
            testMod.CircleSizeMultiplier.Value = 0.5f;

            result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 10,
                OverallDifficulty = 10,
                ApproachRate = 10,
                CircleSize = 10
            });

            Assert.Multiple(() =>
            {
                Assert.That(result.DrainRate, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.OverallDifficulty, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.ApproachRate, Is.GreaterThanOrEqualTo(-10).And.LessThanOrEqualTo(10));
                Assert.That(result.CircleSize, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(10));
                Assert.That(result.DrainRate, Is.EqualTo(5));
                Assert.That(result.OverallDifficulty, Is.EqualTo(5));
                Assert.That(result.ApproachRate, Is.EqualTo(5));
                Assert.That(result.CircleSize, Is.EqualTo(5));
            });

        }

        [Test]
        public void TestChangedSettingsOverrideAppliedDifficultyVarCap()
        {
            //HR settings
            testMod.DrainRateMultiplier.Value = 1.4f;
            testMod.OverallDifficultyMultiplier.Value = 1.4f;
            testMod.ApproachRateMultiplier.Value = 1.4f;
            testMod.CircleSizeMultiplier.Value = 1.3f;

            testMod.DrainCap.Value = 11;
            testMod.OverallDifficultyCap.Value = 11;
            testMod.ApproachRateCap.Value = 11;
            testMod.CircleCap.Value = 11;


            var result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 10,
                OverallDifficulty = 10,
                ApproachRate = 10,
                CircleSize = 10
            });
            Assert.Multiple(() =>
            {
                Assert.That(result.DrainRate, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(11));
                Assert.That(result.OverallDifficulty, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(11));
                Assert.That(result.ApproachRate, Is.GreaterThanOrEqualTo(-10).And.LessThanOrEqualTo(11));
                Assert.That(result.CircleSize, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(11));
                Assert.That(result.DrainRate, Is.EqualTo(11));
                Assert.That(result.OverallDifficulty, Is.EqualTo(11));
                Assert.That(result.ApproachRate, Is.EqualTo(11));
                Assert.That(result.CircleSize, Is.EqualTo(11));
            });

            testMod.DrainRateMultiplier.Value = 2f;
            testMod.OverallDifficultyMultiplier.Value = 1.4f;
            testMod.ApproachRateMultiplier.Value = 0.4f;
            testMod.CircleSizeMultiplier.Value = 1f;

            testMod.DrainCap.Value = 7;
            testMod.OverallDifficultyCap.Value = 8;
            testMod.ApproachRateCap.Value = 2;
            testMod.CircleCap.Value = 6;

            result = applyDifficulty(new BeatmapDifficulty
            {
                DrainRate = 5.5f,
                OverallDifficulty = 4.6f,
                ApproachRate = 8,
                CircleSize = 4.3f
            });

            Assert.Multiple(() =>
            {
                Assert.That(result.DrainRate, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(7));
                Assert.That(result.OverallDifficulty, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(8));
                Assert.That(result.ApproachRate, Is.GreaterThanOrEqualTo(-10).And.LessThanOrEqualTo(2));
                Assert.That(result.CircleSize, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(6));
                Assert.That(result.DrainRate, Is.EqualTo(7));
                Assert.That(result.OverallDifficulty, Is.EqualTo(1.4f * 4.6f));
                Assert.That(result.ApproachRate, Is.EqualTo(2));
                Assert.That(result.CircleSize, Is.EqualTo(4.3f));

            });

        }


        private class TestOsuModDifficultyScalar : OsuModDifficultyScalar
        {
        }


        /// <summary>
        /// Applies a <see cref="BeatmapDifficulty"/> to the mod and returns a new <see cref="BeatmapDifficulty"/>
        /// representing the result if the mod were applied to a fresh <see cref="BeatmapDifficulty"/> instance.
        /// </summary>
        private BeatmapDifficulty applyDifficulty(BeatmapDifficulty difficulty)
        {
            // ensure that ReadFromDifficulty doesn't pollute the values.
            var newDifficulty = difficulty.Clone();

            testMod.ReadFromDifficulty(difficulty);

            testMod.ApplyToDifficulty(newDifficulty);
            return newDifficulty;
        }

        private class TestRuleset : Ruleset
        {
            public override IEnumerable<Mod> GetModsFor(ModType type)
            {
                if (type == ModType.DifficultyIncrease)
                    yield return new TestOsuModDifficultyScalar();
            }

            public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            {
                throw new System.NotImplementedException();
            }

            public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap)
            {
                throw new System.NotImplementedException();
            }

            public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap)
            {
                throw new System.NotImplementedException();
            }

            public override string Description => string.Empty;
            public override string ShortName => string.Empty;
        }
    }
}
