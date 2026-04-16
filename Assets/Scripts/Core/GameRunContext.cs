using System;

namespace Core
{
    public sealed class GameRunContext
    {
        public GameRunContext(GameRoot root, GameSceneContext sceneContext, GameSession session)
        {
            Root = root;
            SceneContext = sceneContext;
            Session = session;
        }

        public GameRoot Root { get; }
        public GameSceneContext SceneContext { get; }
        public GameSession Session { get; }

        public bool IsValid()
        {
            return Root != null
                   && SceneContext != null
                   && Session != null
                   && Session.SelectedPlayer != null;
        }
    }
}
