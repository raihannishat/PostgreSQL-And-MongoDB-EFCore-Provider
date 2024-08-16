namespace EFCoreForPostgreSQLAndMongoDB.Entities;

public class User : IEntity
{
    [BsonId]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("address")]
    public Address? Address { get; set; }

    [BsonElement("contacts")]
    public List<Phone>? Contacts { get; set; }
}

public class Address
{
    [BsonElement("street")]
    public string? Street { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("state")]
    public string? State { get; set; }

    [BsonElement("zip")]
    public string? Zip { get; set; }
}

public class Phone
{
    [BsonElement("type")]
    public string? Type { get; set; }

    [BsonElement("number")]
    public string? Number { get; set; }
}