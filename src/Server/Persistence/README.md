# Persistence boundary

Production/local-server database target: PostgreSQL.

The DbContext, entity configurations, migrations, transaction helpers, and repository implementations will live here. Business modules must not depend on HTTP concerns.
