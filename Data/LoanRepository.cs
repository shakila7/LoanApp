using LoanApp.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;

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

        public IEnumerable<LoanApplication> GetAll() 
        {
            var loanApplicationList = new List<LoanApplication>();
            using var conn = GetConn();
            using var cmd = new SqlCommand("SELECT * FROM LoanApplication ORDER BY CreatedAt DESC", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) 
            {
                loanApplicationList.Add(Map(reader));
            }
            return loanApplicationList;
        }

        public LoanApplication GetById(int id) 
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand("SELECT * FROM LoanApplication WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return Map(reader);
            return null;
        }
        public int Create(LoanApplication model)
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
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }
        public void UpdateStatus(int id, string status)
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand("UPDATE LoanApplication SET Status=@Status, UpdatedAt=@UpdatedAt WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        private LoanApplication Map(SqlDataReader rdr)
        {
            return new LoanApplication
            {
                Id = (int)rdr["Id"],
                CustomerName = rdr["CustomerName"].ToString(),
                NicPassport = rdr["NicPassport"].ToString(),
                LoanType = rdr["LoanType"].ToString(),
                InterestRate = (decimal)rdr["InterestRate"],
                LoanAmount = (decimal)rdr["LoanAmount"],
                DurationMonths = (int)rdr["DurationMonths"],
                Status = rdr["Status"].ToString(),
                CreatedAt = (DateTime)rdr["CreatedAt"]
            };
        }
    }
}
