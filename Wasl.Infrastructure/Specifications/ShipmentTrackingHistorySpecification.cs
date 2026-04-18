namespace Wasl.Infrastructure.Specifications;

public class ShipmentTrackingHistorySpecification : BaseSpecification<ShipmentTrackingHistory>
{
    public ShipmentTrackingHistorySpecification() : base()
    {
        AddIncludes();
    }
    public ShipmentTrackingHistorySpecification(Guid id) : base(rt => rt.Id == id)
    {
        AddIncludes();
    }
    public ShipmentTrackingHistorySpecification(Expression<Func<ShipmentTrackingHistory, bool>> expression) : base(expression)
    {
        AddIncludes();
    }
    void AddIncludes()
    {
        Includes.Add(o => o.Order);


    }
}
