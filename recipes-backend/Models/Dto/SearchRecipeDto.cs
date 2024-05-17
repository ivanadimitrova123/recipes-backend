namespace recipes_backend.Models.Dto
{
    public class SearchRecipeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }

        public SearchRecipeDto(long id, string name, string picture)
        {
            Id = id;
            Name = name;
            Picture = picture;
        }
    }
}
