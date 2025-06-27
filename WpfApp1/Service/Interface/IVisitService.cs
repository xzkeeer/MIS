public interface IVisitService
{
    Task<List<Visit>> GetAllVisitsAsync();
    Task<Visit> GetVisitByIdAsync(int visitId);
    Task<int> CreateVisitAsync(Visit visit);
    Task<bool> UpdateVisitAsync(Visit visit);
    Task<bool> DeleteVisitAsync(int visitId);
}