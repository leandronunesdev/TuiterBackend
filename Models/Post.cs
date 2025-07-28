using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Representa uma postagem no estilo Twitter
/// </summary>
public class Post
{
    /// <summary>
    /// Identificador único da postagem
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Identificador do usuário que criou a postagem
    /// </summary>
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    /// <summary>
    /// Conteúdo da postagem com limite de 280 caracteres
    /// </summary>
    [BsonElement("content")]
    [Required(ErrorMessage = "O conteúdo é obrigatório")]
    [StringLength(280, ErrorMessage = "O conteúdo deve ter no máximo 280 caracteres")]
    public string Content { get; set; }

    /// <summary>
    /// Data e hora de criação da postagem
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização da postagem (opcional)
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Número de curtidas da postagem
    /// </summary>
    [BsonElement("likesCount")]
    public int LikesCount { get; set; }

    /// <summary>
    /// Indica se a postagem foi excluída logicamente
    /// </summary>
    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; }
}