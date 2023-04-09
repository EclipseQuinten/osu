using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Catch.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using System;



namespace osu.Game.Rulesets.Catch.Difficulty.Skills
{
    public class Stamina : StrainDecaySkill
    {

        // Error positioning w.r.t. middle of fruit
        private const float absolute_player_positioning_error = 16f;

        private const float normalized_hitobject_radius = 41.0f;

        private const float distance_factor = 1.0f;

        private const float time_factor = 0.2f;

        private const float no_hyper_dash_bonus = 2.0f;

        private float? lastPlayerPosition;

        public Stamina(Mod[] mods) : base(mods)
        {
        }

        protected override double SkillMultiplier => 1;

        protected override double StrainDecayBase => 0.0;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            var catchCurrent = (CatchDifficultyHitObject)current;
            return 0f;
        }

    }
    
}