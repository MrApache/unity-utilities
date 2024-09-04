using System;

namespace Irisu.Utilities
{
    public sealed class InvalidAssetType : Exception
    {
        public InvalidAssetType(object asset, Type excepted)
            : base($"Invalid asset type '{asset.GetType().Name}'. Should be '{excepted.Name}'")
        { }
    }
}