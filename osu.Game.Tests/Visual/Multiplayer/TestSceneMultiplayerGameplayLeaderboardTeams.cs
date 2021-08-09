// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Framework.Utils;
using osu.Game.Configuration;
using osu.Game.Online.API;
using osu.Game.Online.Multiplayer.MatchTypes.TeamVersus;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Screens.Play.HUD;
using osu.Game.Tests.Visual.OnlinePlay;
using osu.Game.Tests.Visual.Spectator;
using osu.Game.Users;

namespace osu.Game.Tests.Visual.Multiplayer
{
    public class TestSceneMultiplayerGameplayLeaderboardTeams : MultiplayerTestScene
    {
        private static IEnumerable<int> users => Enumerable.Range(0, 16);

        public new TestSceneMultiplayerGameplayLeaderboard.TestMultiplayerSpectatorClient SpectatorClient =>
            (TestSceneMultiplayerGameplayLeaderboard.TestMultiplayerSpectatorClient)OnlinePlayDependencies?.SpectatorClient;

        protected override OnlinePlayTestSceneDependencies CreateOnlinePlayDependencies() => new TestDependencies();

        protected class TestDependencies : MultiplayerTestSceneDependencies
        {
            protected override TestSpectatorClient CreateSpectatorClient() => new TestSceneMultiplayerGameplayLeaderboard.TestMultiplayerSpectatorClient();
        }

        private MultiplayerGameplayLeaderboard leaderboard;

        protected override Room CreateRoom()
        {
            var room = base.CreateRoom();
            room.Type.Value = MatchType.TeamVersus;
            return room;
        }

        [SetUpSteps]
        public override void SetUpSteps()
        {
            AddStep("set local user", () => ((DummyAPIAccess)API).LocalUser.Value = LookupCache.GetUserAsync(1).Result);

            AddStep("create leaderboard", () =>
            {
                leaderboard?.Expire();

                OsuScoreProcessor scoreProcessor;
                Beatmap.Value = CreateWorkingBeatmap(Ruleset.Value);

                var playable = Beatmap.Value.GetPlayableBeatmap(Ruleset.Value);

                foreach (var user in users)
                {
                    SpectatorClient.StartPlay(user, Beatmap.Value.BeatmapInfo.OnlineBeatmapID ?? 0);
                    var roomUser = OnlinePlayDependencies.Client.AddUser(new User { Id = user });

                    roomUser.MatchState = new TeamVersusUserState
                    {
                        TeamID = RNG.Next(0, 2)
                    };
                }

                // Todo: This is REALLY bad.
                Client.CurrentMatchPlayingUserIds.AddRange(users);

                Children = new Drawable[]
                {
                    scoreProcessor = new OsuScoreProcessor(),
                };

                scoreProcessor.ApplyBeatmap(playable);

                LoadComponentAsync(leaderboard = new MultiplayerGameplayLeaderboard(scoreProcessor, users.ToArray())
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }, Add);
            });

            AddUntilStep("wait for load", () => leaderboard.IsLoaded);
            AddUntilStep("wait for user population", () => Client.CurrentMatchPlayingUserIds.Count > 0);
        }

        [Test]
        public void TestScoreUpdates()
        {
            AddRepeatStep("update state", () => SpectatorClient.RandomlyUpdateState(), 100);
            AddToggleStep("switch compact mode", expanded => leaderboard.Expanded.Value = expanded);
        }
    }
}
