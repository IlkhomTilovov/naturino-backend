namespace Naturino.Domain.Common;

public interface IAuditable
{
    Guid? CreatedBy { get; set; }
    Guid? UpdatedBy { get; set; }
}
