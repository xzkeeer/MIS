public interface IPatientService
{
    Task<List<Patient>> GetAllPatientsAsync();
    Task<Patient> GetPatientByIdAsync(int patientId);
    Task<int> CreatePatientAsync(Patient patient);
    Task<bool> UpdatePatientAsync(Patient patient);
    Task<bool> DeletePatientAsync(int patientId);
}