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
    public class DirectionChange : StrainDecaySkill
    {

        // Error positioning w.r.t. middle of fruit
        private const float absolute_player_positioning_error = 16f;

        private const float normalized_hitobject_radius = 41.0f;

        private float? lastPlayerPosition;

        private Direction lastDirectionMoved;

        private const float direction_change_bonus = 1.2f;

        private const float no_direction_change_malus = 0.8f;

        private const float movement_stop_start_bonus = 1.0f;


        enum Direction 
    {
        LEFT,
        STANDSTILL, 
        RIGHT
    }

        public DirectionChange(Mod[] mods) : base(mods)
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
            Direction currentDirection;

            if (distanceMoved > 0) {
                currentDirection = Direction.LEFT;
            } else if (distanceMoved == 0) {
                currentDirection = Direction.STANDSTILL;
            } else {
                currentDirection = Direction.RIGHT;
            }

            float weight;

            // If it is required to move in the current setup
            if (currentDirection == lastDirectionMoved) {
                weight = no_direction_change_malus;
            } else if (currentDirection != lastDirectionMoved && (currentDirection == Direction.STANDSTILL || lastDirectionMoved == Direction.STANDSTILL)) {
                weight = movement_stop_start_bonus;
            } else {
                weight = direction_change_bonus;
            }


            // Set last direction moved
            lastDirectionMoved = currentDirection;
            Console.WriteLine("WEIGHT" + weight);
            return weight;
        }

    }
    
}