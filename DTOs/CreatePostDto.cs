using System.ComponentModel.DataAnnotations;

public class CreatePostDto
{
  [Required]
  public string UserId { get; set; }

  [Required]
  [MaxLength(280)]
  public string Content { get; set; }
}
