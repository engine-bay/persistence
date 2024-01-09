namespace EngineBay.Persistence.Tests
{
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class BaseDatabaseConfigurationTests
    {
        [Fact]
        public void IsDatabaseResetInMemoryReturnsTrue()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, "InMemory");

            Assert.True(BaseDatabaseConfiguration.IsDatabaseReset());
        }

        [Fact]
        public void IsDatabaseResetResetTrueReturnsTrue()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, null);
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESET, "true");

            Assert.True(BaseDatabaseConfiguration.IsDatabaseReset());
        }

        [Fact]
        public void IsDatabaseResetResetFalseReturnsFalse()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, null);
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESET, "false");

            Assert.False(BaseDatabaseConfiguration.IsDatabaseReset());
        }

        [Fact]
        public void IsDatabaseResetEnvironmentVariableNotSetReturnsFalse()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, null);
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESET, null);

            Assert.False(BaseDatabaseConfiguration.IsDatabaseReset());
        }

        [Fact]
        public void IsDatabaseReseededWhenEnvironmentVariableNotSetReturnsFalse()
        {
            // Arrange
            bool isDatabaseReseeded = false;
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESEED, null);

            // Act
            isDatabaseReseeded = BaseDatabaseConfiguration.IsDatabaseReseeded();

            // Assert
            Assert.False(isDatabaseReseeded);
        }

        [Fact]
        public void IsDatabaseReseededWhenEnvironmentVariableIsFalseReturnsFalse()
        {
            // Arrange
            bool isDatabaseReseeded = false;
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESEED, "false");

            // Act
            isDatabaseReseeded = BaseDatabaseConfiguration.IsDatabaseReseeded();

            // Assert
            Assert.False(isDatabaseReseeded);
        }

        [Fact]
        public void IsDatabaseReseededWhenEnvironmentVariableIsTrueReturnsTrue()
        {
            // Arrange
            bool isDatabaseReseeded = false;
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESEED, "true");

            // Act
            isDatabaseReseeded = BaseDatabaseConfiguration.IsDatabaseReseeded();

            // Assert
            Assert.True(isDatabaseReseeded);
        }

        [Fact]
        public void ShouldExitAfterMigrationsWithEnvironmentVariableSetToTrueReturnsTrue()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS, "true");

            var exitAfterMigrations = BaseDatabaseConfiguration.ShouldExitAfterMigrations();

            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS, null);

            Assert.True(exitAfterMigrations);
        }

        [Fact]
        public void ShouldExitAfterMigrationsWithEnvironmentVariableSetToFalseReturnsFalse()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS, "false");

            var exitAfterMigrations = BaseDatabaseConfiguration.ShouldExitAfterMigrations();

            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS, null);

            Assert.False(exitAfterMigrations);
        }

        [Fact]
        public void ShouldExitAfterMigrationsWithEnvironmentVariableSetToEmptyStringReturnsFalse()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS, string.Empty);

            var exitAfterMigrations = BaseDatabaseConfiguration.ShouldExitAfterMigrations();

            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS, null);

            Assert.False(exitAfterMigrations);
        }

        [Fact]
        public void ShouldExitAfterMigrationsWithEnvironmentVariableNotSetReturnsFalse()
        {
            var exitAfterMigrations = BaseDatabaseConfiguration.ShouldExitAfterMigrations();

            Assert.False(exitAfterMigrations);
        }

        [Fact]
        public void ShouldExitAfterSeedingNoEnvironmentVariableShouldReturnFalse()
        {
            // Act
            var exitAfterSeeding = BaseDatabaseConfiguration.ShouldExitAfterSeeding();

            // Assert
            Assert.False(exitAfterSeeding);
        }

        [Fact]
        public void ShouldExitAfterSeedingTrueEnvironmentVariableShouldReturnTrue()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERSEEDING, "true");

            // Act
            var exitAfterSeeding = BaseDatabaseConfiguration.ShouldExitAfterSeeding();

            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERSEEDING, null);

            // Assert
            Assert.True(exitAfterSeeding);
        }

        [Fact]
        public void ShouldExitAfterSeedingFalseEnvironmentVariableShouldReturnFalse()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERSEEDING, "false");

            // Act
            var exitAfterSeeding = BaseDatabaseConfiguration.ShouldExitAfterSeeding();

            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERSEEDING, null);

            // Assert
            Assert.False(exitAfterSeeding);
        }

        [Theory]
        [InlineData(LogLevel.Trace, true)]
        [InlineData(LogLevel.Debug, false)]
        [InlineData(LogLevel.Information, false)]
        [InlineData(LogLevel.Warning, false)]
        [InlineData(LogLevel.Error, false)]
        [InlineData(LogLevel.Critical, false)]
        [InlineData(LogLevel.None, false)]
        public void IsDetailedDatabaseLoggingEnabledEnvironmentVariableFalseShouldReturnFalse(
            LogLevel loglevel,
            bool expectedIsDetailedDatabaseLoggingEnabled)
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.LOGGINGLEVEL, loglevel.ToString());
            Assert.Equal(BaseDatabaseConfiguration.IsDetailedDatabaseLoggingEnabled(), expectedIsDetailedDatabaseLoggingEnabled);
        }
    }
}
