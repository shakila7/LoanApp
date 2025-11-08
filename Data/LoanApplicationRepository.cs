using LoanApp.Models;
using Microsoft.Data.SqlClient;


namespace LoanApp.Data
{
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private readonly string _connectionString;
        public LoanApplicationRepository(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection GetConn() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<LoanApplicationViewModel>> GetAll() 
        {
            var loanApplicationList = new List<LoanApplicationViewModel>();
            using var conn = GetConn();
            using var cmd = new SqlCommand(@"SELECT LoanApplication.Id,
                            CustomerName, NicPassport, LoanAmount, DurationMonths, Status, LoanType.Name as LoanType, LoanApplication.InterestRate
                            FROM LoanApplication left join LoanType on LoanApplication.LoanTypeId = LoanType.Id ORDER BY UpdatedAt DESC", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read()) 
            {
                loanApplicationList.Add(Map(reader));
            }
            return loanApplicationList;
        }

        public async Task<LoanApplicationViewModel> GetById(int id) 
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand(@"SELECT LoanApplication.Id,
                            CustomerName, NicPassport, LoanAmount, DurationMonths, Status, LoanType.Name as LoanType, LoanApplication.InterestRate
                            FROM LoanApplication left join LoanType on LoanApplication.LoanTypeId = LoanType.Id WHERE LoanApplication.Id = @Id", conn);
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
            INSERT INTO LoanApplication (CustomerName, NicPassport, LoanTypeId, InterestRate, LoanAmount, DurationMonths, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@CustomerName, @NicPassport, @LoanTypeId,@InterestRate, @LoanAmount, @DurationMonths, @Status, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
            SELECT SCOPE_IDENTITY();", conn);

            cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
            cmd.Parameters.AddWithValue("@NicPassport", model.NicPassport);
            cmd.Parameters.AddWithValue("@LoanTypeId", model.LoanTypeId);
            cmd.Parameters.AddWithValue("@InterestRate", model.InterestRate);
            cmd.Parameters.AddWithValue("@LoanAmount", model.LoanAmount);
            cmd.Parameters.AddWithValue("@DurationMonths", model.DurationMonths);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }
        public async Task UpdateStatus(int id, string status, string user)
        {
            using var conn = GetConn();
            using var cmd = new SqlCommand("UPDATE LoanApplication SET Status=@Status, UpdatedAt=@UpdatedAt, UpdatedBy=@UpdatedBy WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedBy", user);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<LoanType>> GetAllLoanTypes()
        {
            var loanTypes = new List<LoanType>();
            using var conn = GetConn();
            using var cmd = new SqlCommand("SELECT * FROM LoanType", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                loanTypes.Add(MapLoanType(reader));
            }
            return loanTypes;
        }
        private LoanApplicationViewModel Map(SqlDataReader rdr)
        {
            return new LoanApplicationViewModel
            {
                Id = Convert.ToInt32(rdr["Id"]),
                CustomerName = Convert.ToString(rdr["CustomerName"]),
                NicPassport = Convert.ToString(rdr["NicPassport"]),
                LoanType = Convert.ToString(rdr["LoanType"]),
                InterestRate = Convert.ToDecimal(rdr["InterestRate"]),
                LoanAmount = Convert.ToDecimal(rdr["LoanAmount"]),
                DurationMonths = Convert.ToInt32(rdr["DurationMonths"]),
                Status = Convert.ToString(rdr["Status"]),
            };
        }

        private LoanType MapLoanType(SqlDataReader rdr)
        {
            return new LoanType
            {
                Id = Convert.ToInt32(rdr["Id"]),
                Name = Convert.ToString(rdr["Name"]),
                InterestRate = Convert.ToDecimal(rdr["InterestRate"])
            };
        }
    }
}
