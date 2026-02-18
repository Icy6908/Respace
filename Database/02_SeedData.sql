-- Clear any existing data to start fresh
DELETE FROM UserMemberships;
DELETE FROM MembershipPlans;

-- Use IDENTITY_INSERT if your PlanId is an identity column
SET IDENTITY_INSERT MembershipPlans ON;

INSERT INTO MembershipPlans (PlanId, PlanName, MonthlyFee, BookingDiscountPct, PointsMultiplier, IsActive)
VALUES 
(1, 'Free', 0.00, 0.00, 1.00, 1),
(2, 'Plus', 9.90, 0.05, 1.20, 1),
(3, 'Pro', 19.90, 0.10, 1.50, 1);

SET IDENTITY_INSERT MembershipPlans OFF;