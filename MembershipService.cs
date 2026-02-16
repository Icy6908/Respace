using Respace.App_Code;
using System;
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

                if (dt.Rows.Count == 0) return info;

                var r = dt.Rows[0];
                info.PlanName = r["PlanName"].ToString();
                info.BookingDiscountPct = Convert.ToDecimal(r["BookingDiscountPct"]);
                info.PointsMultiplier = Convert.ToDecimal(r["PointsMultiplier"]);
                return info;
            }
            catch
            {
                return info; // fallback to Free
            }
        }

        public static void ActivatePlan(int userId, int planId, bool autoRenew)
        {
            Db.Execute("UPDATE UserMemberships SET IsActive=0 WHERE UserId=@U",
                new SqlParameter("@U", userId));

            Db.Execute(@"
                INSERT INTO UserMemberships (UserId, PlanId, StartDate, EndDate, IsActive, AutoRenew)
                VALUES (@U, @P, GETDATE(), NULL, 1, @A)
            ",
            new SqlParameter("@U", userId),
            new SqlParameter("@P", planId),
            new SqlParameter("@A", autoRenew ? 1 : 0));
        }
    }
}
