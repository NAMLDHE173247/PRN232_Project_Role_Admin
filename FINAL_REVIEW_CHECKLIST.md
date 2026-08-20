# Final Review Checklist

## Architecture

- [ ] Controller does not contain business logic.
- [ ] Service owns business rules and status transitions.
- [ ] Repository owns database queries.
- [ ] MVC calls the API through `AdminApiClient` and never accesses `DbContext`.

## Security

- [ ] JWT validation checks issuer, audience, signing key and lifetime.
- [ ] Admin endpoints require the `Admin` role.
- [ ] Passwords are stored as BCrypt hashes.
- [ ] JWT and Admin secrets are provided through environment variables.
- [ ] No `.env` or local Development secrets are committed.
- [ ] Missing token returns `401`; insufficient role returns `403`.

## Admin Features

- [ ] Dashboard and period statistics.
- [ ] User approval, block and unblock.
- [ ] Product hide and unhide.
- [ ] Order list and details.
- [ ] Dispute assign, resolve and reject.
- [ ] Review hide and unhide.
- [ ] Feedback monitoring.
- [ ] Audit log pagination and action history.

## Deployment

- [ ] `dotnet build EbayClone.sln` passes.
- [ ] API `/health` returns `Healthy`.
- [ ] Docker Compose variables are configured from `.env`.
- [ ] SQL Server, API, MVC and Nginx are checked with Docker Desktop running.
- [ ] GitHub Actions restore, build, test and Docker image builds pass.

## Demo rehearsal

- [ ] Login as Admin.
- [ ] Show Dashboard.
- [ ] Approve or block a user.
- [ ] Hide a product and a review.
- [ ] Resolve a dispute.
- [ ] Open Audit Logs and explain the actor/action/resource fields.
