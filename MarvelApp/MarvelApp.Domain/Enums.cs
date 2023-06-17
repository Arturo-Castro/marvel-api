namespace MarvelApp.Domain
{
    public class Enums
    {
        public enum TeamCreationError
        {
            None = 0,
            CharacterNotFound = 1,
            CharacterAlreadyHasTeam = 2,
            TeamAlreadyExists = 3
        }
    }
}
