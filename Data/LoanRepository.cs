using LoanApp.Models;
using Microsoft.Data.SqlClient;

namespace LoanApp.Data
{
    public class LoanRepository : ILoanRepository
    {
        private readonly string _connectionString;
        public LoanRepository(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection GetConn() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<LoanApplication>> GetAll() 
        {
            var loanApplicationList = new List<LoanApplication>();
            using var conn = GetConn();
            using var cmd = new SqlCommand("SELECT * FROM LoanApplication ORDER BY CreatedAt DESC", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read()) 
            {
                loanApplicationList.Add(Map(reader));
            }
            return loanApplicationList;
        }

        public async Task<LoanApplication> GetById(int id) 
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand("SELECT * FROM LoanApplication WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read()) return Map(reader);
            return null;
        }
        public async Task<int> Create(LoanApplication model)
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand(@"
            INSERT INTO LoanApplication (CustomerName, NicPassport, LoanType, InterestRate, LoanAmount, DurationMonths, Status, CreatedAt, UpdatedAt)
            VALUES (@CustomerName, @NicPassport, @LoanType, @InterestRate, @LoanAmount, @DurationMonths, @Status, @CreatedAt, @UpdatedAt);
            SELECT SCOPE_IDENTITY();", conn);

            cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
            cmd.Parameters.AddWithValue("@NicPassport", model.NicPassport);
            cmd.Parameters.AddWithValue("@LoanType", model.LoanType);
            cmd.Parameters.AddWithValue("@InterestRate", model.InterestRate);
            cmd.Parameters.AddWithValue("@LoanAmount", model.LoanAmount);
            cmd.Parameters.AddWithValue("@DurationMonths", model.DurationMonths);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }
        public async Task UpdateStatus(int id, string status)
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand("UPDATE LoanApplication SET Status=@Status, UpdatedAt=@UpdatedAt WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        private LoanApplication Map(SqlDataReader rdr)
        {
            return new LoanApplication
            {
                Id = Convert.ToInt32(rdr["Id"]),
                CustomerName = Convert.ToString(rdr["CustomerName"]),
                NicPassport = Convert.ToString(rdr["NicPassport"]),
                LoanType = Convert.ToString(rdr["LoanType"]),
                InterestRate = Convert.ToDecimal(rdr["InterestRate"]),
                LoanAmount = Convert.ToDecimal(rdr["LoanAmount"]),
                DurationMonths = Convert.ToInt32(rdr["DurationMonths"]),
                Status = Convert.ToString(rdr["Status"]),
                CreatedAt = Convert.ToDateTime(rdr["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(rdr["UpdatedAt"])
            };
        }
    }
}
