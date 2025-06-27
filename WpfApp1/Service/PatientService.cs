using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Dapper;

public class PatientService : BaseRepository, IPatientService
{
    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        try
        {
            var query = @"SELECT p.*, u.first_name, u.last_name 
                        FROM patients p 
                        JOIN users u ON p.user_id = u.user_id";
            return (await QueryAsync<Patient>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patients: {ex.Message}");
            return new List<Patient>();
        }
    }

    public async Task<Patient> GetPatientByIdAsync(int patientId)
    {
        try
        {
            var query = @"SELECT p.*, u.first_name, u.last_name 
                        FROM patients p 
                        JOIN users u ON p.user_id = u.user_id
                        WHERE p.patient_id = @PatientId";
            return await QueryFirstOrDefaultAsync<Patient>(query, new { PatientId = patientId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patient: {ex.Message}");
            return null;
        }
    }

    public async Task<int> CreatePatientAsync(Patient patient)
    {
        try
        {
            var query = @"INSERT INTO patients 
                        (user_id, birth_date, gender, phone, address, insurance_policy) 
                        VALUES (@UserId, @BirthDate, @Gender, @Phone, @Address, @InsurancePolicy)
                        RETURNING patient_id";
            return await ExecuteScalarAsync<int>(query, patient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating patient: {ex.Message}");
            return -1;
        }
    }

    public async Task<bool> UpdatePatientAsync(Patient patient)
    {
        try
        {
            var query = @"UPDATE patients SET 
                        birth_date = @BirthDate,
                        gender = @Gender,
                        phone = @Phone,
                        address = @Address,
                        insurance_policy = @InsurancePolicy
                        WHERE patient_id = @PatientId";

            var affected = await ExecuteAsync(query, patient);
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeletePatientAsync(int patientId)
    {
        try
        {
            var affected = await ExecuteAsync(
                "DELETE FROM patients WHERE patient_id = @PatientId",
                new { PatientId = patientId });
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting patient: {ex.Message}");
            return false;
        }
    }
}