using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Respace
{
    public static class MembershipService
    {
        public class MembershipInfo
        {
            public string PlanName { get; set; } = "Free";
            public decimal BookingDiscountPct { get; set; } = 0m;
            public decimal PointsMultiplier { get; set; } = 1.00m;
        }

        public static MembershipInfo GetActiveMembership(int userId)
        {
            var info = new MembershipInfo();

            try
            {
                var dt = Db.Query(@"
                    SELECT TOP 1 p.PlanName, p.BookingDiscountPct, p.PointsMultiplier
                    FROM UserMemberships um
                    INNER JOIN MembershipPlans p ON p.PlanId = um.PlanId
                    WHERE um.UserId=@U AND um.IsActive=1
                    ORDER BY um.StartDate DESC
                ", new SqlParameter("@U", userId));

                if (dt == null || dt.Rows.Count == 0) return info;

                var r = dt.Rows[0];
                info.PlanName = r["PlanName"].ToString();
                info.BookingDiscountPct = Convert.ToDecimal(r["BookingDiscountPct"]);
                info.PointsMultiplier = Convert.ToDecimal(r["PointsMultiplier"]);
                return info;
            }
            catch
            {
                return info; // fallback to Free on error
            }
        }

        public static void ActivatePlan(int userId, int planId, bool autoRenew)
        {
            // 1. Verify the requested plan exists
            var existsObj = Db.Scalar("SELECT TOP 1 1 FROM MembershipPlans WHERE PlanId = @P",
                new SqlParameter("@P", planId));

            if (existsObj == null || existsObj == DBNull.Value)
            {
                throw new InvalidOperationException($"Membership plan {planId} does not exist.");
            }

            // 2. Wrap deactivation and activation in a single execution
            // We use a transaction (BEGIN TRAN/COMMIT) to ensure data integrity
            string sql = @"
                BEGIN TRANSACTION;
                BEGIN TRY
                    -- Deactivate existing memberships
                    UPDATE UserMemberships SET IsActive=0 WHERE UserId=@U;

                    -- Insert the new active plan
                    INSERT INTO UserMemberships (UserId, PlanId, StartDate, EndDate, IsActive, AutoRenew)
                    VALUES (@U, @P, GETDATE(), NULL, 1, @A);

                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;
                    THROW;
                END CATCH";

            Db.Execute(sql,
                new SqlParameter("@U", userId),
                new SqlParameter("@P", planId),
                new SqlParameter("@A", autoRenew ? 1 : 0));
        }
        public static void DowngradeToFree(int userId)
        {
            // Simply call ActivatePlan with ID 1 (Free) and AutoRenew false
            ActivatePlan(userId, 1, false);
        }
    }
}