using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Dapper;

public class VisitService : BaseRepository, IVisitService
{
    public async Task<List<Visit>> GetAllVisitsAsync()
    {
        try
        {
            var query = @"SELECT v.*, 
                        p.patient_id, p.insurance_policy,
                        u1.first_name as patient_first_name, u1.last_name as patient_last_name,
                        s.staff_id, s.position,
                        u2.first_name as staff_first_name, u2.last_name as staff_last_name
                        FROM visits v
                        JOIN patients p ON v.patient_id = p.patient_id
                        JOIN users u1 ON p.user_id = u1.user_id
                        JOIN staff s ON v.staff_id = s.staff_id
                        JOIN users u2 ON s.user_id = u2.user_id";
            return (await QueryAsync<Visit>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting visits: {ex.Message}");
            return new List<Visit>();
        }
    }

    public async Task<Visit> GetVisitByIdAsync(int visitId)
    {
        try
        {
            var query = @"SELECT v.*, 
                        p.patient_id, p.insurance_policy,
                        u1.first_name as patient_first_name, u1.last_name as patient_last_name,
                        s.staff_id, s.position,
                        u2.first_name as staff_first_name, u2.last_name as staff_last_name
                        FROM visits v
                        JOIN patients p ON v.patient_id = p.patient_id
                        JOIN users u1 ON p.user_id = u1.user_id
                        JOIN staff s ON v.staff_id = s.staff_id
                        JOIN users u2 ON s.user_id = u2.user_id
                        WHERE v.visit_id = @VisitId";
            return await QueryFirstOrDefaultAsync<Visit>(query, new { VisitId = visitId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting visit: {ex.Message}");
            return null;
        }
    }

    public async Task<int> CreateVisitAsync(Visit visit)
    {
        try
        {
            var query = @"INSERT INTO visits 
                        (patient_id, staff_id, visit_date, status) 
                        VALUES (@PatientId, @StaffId, @VisitDate, @Status)
                        RETURNING visit_id";
            return await ExecuteScalarAsync<int>(query, visit);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating visit: {ex.Message}");
            return -1;
        }
    }

    public async Task<bool> UpdateVisitAsync(Visit visit)
    {
        try
        {
            var query = @"UPDATE visits SET 
                        patient_id = @PatientId,
                        staff_id = @StaffId,
                        visit_date = @VisitDate,
                        status = @Status
                        WHERE visit_id = @VisitId";

            var affected = await ExecuteAsync(query, visit);
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating visit: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteVisitAsync(int visitId)
    {
        try
        {
            var affected = await ExecuteAsync(
                "DELETE FROM visits WHERE visit_id = @VisitId",
                new { VisitId = visitId });
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting visit: {ex.Message}");
            return false;
        }
    }
}