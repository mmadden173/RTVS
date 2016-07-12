﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.R.Components.Application.Configuration;
using Microsoft.R.Host.Client;
using Microsoft.UnitTests.Core.XUnit;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.R.Package.ProjectSystem;
using Microsoft.VisualStudio.R.Package.ProjectSystem.Configuration;
using Microsoft.VisualStudio.R.Package.Sql;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;
using static System.FormattableString;

namespace Microsoft.VisualStudio.R.Package.Test.Sql {
    [ExcludeFromCodeCoverage]
    public class CommandTest {
        [Test]
        [Category.Sql]
        public async Task AddDbConnectionCommand() {
            var coll = new ConfigurationSettingCollection();
            coll.Add(new ConfigurationSetting("dbConnection1", "1", ConfigurationSettingValueType.String));
            coll.Add(new ConfigurationSetting("dbConnection2", "1", ConfigurationSettingValueType.String));
            coll.Add(new ConfigurationSetting("dbConnection4", "1", ConfigurationSettingValueType.String));

            var configuredProject = Substitute.For<ConfiguredProject>();
            var ac = Substitute.For<IProjectConfigurationSettingsAccess>();
            ac.Settings.Returns(coll);
            var csp = Substitute.For<IProjectConfigurationSettingsProvider>();
            csp.OpenProjectSettingsAccessAsync(configuredProject).Returns(ac);

            var properties = new ProjectProperties(Substitute.For<ConfiguredProject>());
            var dbcs = Substitute.For<IDbConnectionService>();
            dbcs.EditConnectionString(null).Returns("DSN");

            var session = Substitute.For<IRSession>();
            session.EvaluateAsync(null, REvaluationKind.NoResult).ReturnsForAnyArgs(Task.FromResult(new REvaluationResult()));

            var hier = Substitute.For<IVsHierarchy>();
            hier.GetConfiguredProject().Returns(configuredProject);
            var pss = Substitute.For<IProjectSystemServices>();
            pss.GetSelectedProject<IVsHierarchy>().Returns(hier);

            var cmd = new AddDbConnectionCommand(dbcs, pss, csp, session);
            cmd.Enabled.Should().BeTrue();
            cmd.Invoke(null, IntPtr.Zero, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT);

            coll.Should().HaveCount(4);
            var s = coll.GetSetting("dbConnection3");
            s.Value.Should().Be("DSN");
            s.ValueType.Should().Be(ConfigurationSettingValueType.String);
            s.Category.Should().Be(ConnectionStringEditor.ConnectionStringEditorCategory);
            s.EditorType.Should().Be(ConnectionStringEditor.ConnectionStringEditorName);
            s.Description.Should().Be(Resources.ConnectionStringDescription);

            await session.Received(1).EvaluateAsync(Invariant($"dbConnection3 <- 'DSN'"), REvaluationKind.Mutating);
        }
    }
}
