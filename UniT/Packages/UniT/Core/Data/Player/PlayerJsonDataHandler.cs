namespace UniT.Core.Data.Player
{
    using System;
    using UniT.Core.Data.Base;

    public abstract class PlayerJsonDataHandler : BaseJsonDataHandler
    {
        public override bool CanHandle(Type type)
        {
            return typeof(IPlayerData).IsAssignableFrom(type);
        }
    }
}