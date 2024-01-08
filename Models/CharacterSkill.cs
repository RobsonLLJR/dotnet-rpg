namespace dotnet_rpg.Models
{
    public class CharacterSkill
    {
        public int IdCharacter { get; set; }
        public int IdSkill { get; set; }
        public Character Character { get; set; }
        public Skill Skill { get; set; }
    }
}
