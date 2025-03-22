namespace MapShark.Benchmark.Models.Record
{
    public record Destination(
        int Id,
        string Name,
        string Description,
        DateTime CreatedAt,
        int Field1,
        string Field2,
        double Field3,
        decimal Field4,
        bool Field5,
        DateTime Field6,
        Guid Field7,
        long Field8,
        float Field9,
        char Field10
    );
}
