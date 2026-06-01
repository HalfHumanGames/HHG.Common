namespace HHG.Common.Editor
{
    public interface IPSDSkinner
    {
        public string TorsoBoneName => "Torso Bone";
        public string PelvisBoneName => "Pelvis Bone";
        public float TorsoBlendHalfWidth => 36;
        public float OutlineDetail => 0f;

        public string MapSpriteToBone(string spriteName);
        public bool IsTorsoSprite(string spriteName);
    }
}
