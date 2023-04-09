using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Catch.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using System;

/*

2. Direction changes
    two objects + one direction at all times
    three directions: left, right, standstill

    movementTypes: left -> right
    right -> left
    left -> standstill 
    standstill -> left
    right -> standstill
    standstill -> right
*/

namespace osu.Game.Rulesets.Catch.Difficulty.Skills
{
    public class TravelDistance : StrainDecaySkill
    {

        // Error positioning w.r.t. middle of fruit
        private const float absolute_player_positioning_error = 16f;

        private const float normalized_hitobject_radius = 41.0f;

        private const float distance_factor = 1.0f;

        private const float time_factor = 0.2f;

        private const float no_hyper_dash_bonus = 2.0f;

        private float? lastPlayerPosition;

        public TravelDistance(Mod[] mods) : base(mods)
        {
        }

        protected override double SkillMultiplier => 1;

        protected override double StrainDecayBase => 0.0;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            var catchCurrent = (CatchDifficultyHitObject)current;

            lastPlayerPosition ??= catchCurrent.LastNormalizedPosition;

            float playerPosition = Math.Clamp(
                lastPlayerPosition.Value,
                catchCurrent.NormalizedPosition - (normalized_hitobject_radius - absolute_player_positioning_error),
                catchCurrent.NormalizedPosition + (normalized_hitobject_radius - absolute_player_positioning_error)
            );

            float distanceMoved = playerPosition - lastPlayerPosition.Value;

            // Bonus for non-hyper dashes.
            if (!catchCurrent.LastObject.HyperDash)
                return no_hyper_dash_bonus * (distance_factor * distanceMoved)/(time_factor * catchCurrent.StrainTime);


            Console.WriteLine((distance_factor * distanceMoved) / (time_factor * catchCurrent.StrainTime));
            return (distance_factor * distanceMoved) / (time_factor * catchCurrent.StrainTime);


        }

    }
    
}