﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.Composition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    [Trait("SharingBoundary", "")]
    public class SharingBoundaryWithNestedBoundaryFactoryTests
    {
        [MefFact(CompositionEngines.V2Compat)]
        public void SharingBoundaryWithSameNestedBoundary(IContainer container)
        {
            var root = container.GetExportedValue<Root>();
            var boundaryPart = root.SelfFactory.CreateExport().Value;
            var boundaryPartNested = boundaryPart.SelfFactoryWithSharingBoundary.CreateExport().Value;

            Assert.NotSame(boundaryPart, boundaryPartNested);
            Assert.NotSame(boundaryPart.AnotherSharedValue, boundaryPartNested.AnotherSharedValue);
            Assert.Same(boundaryPart, boundaryPart.AnotherSharedValue.FirstSharedPart);
            Assert.Same(boundaryPartNested, boundaryPartNested.AnotherSharedValue.FirstSharedPart);
        }

        [Export]
        public class Root
        {
            [Import, SharingBoundary("A")]
            public ExportFactory<SharingBoundaryPart> SelfFactory { get; set; } = null!;
        }

        [Export, Shared("A")]
        public class SharingBoundaryPart
        {
            [Import, SharingBoundary("A")]
            public ExportFactory<SharingBoundaryPart> SelfFactoryWithSharingBoundary { get; set; } = null!;

            [Import]
            public AnotherSharedPartInBoundaryA AnotherSharedValue { get; set; } = null!;
        }

        [Export, Shared("A")]
        public class AnotherSharedPartInBoundaryA
        {
            [Import]
            public SharingBoundaryPart FirstSharedPart { get; set; } = null!;
        }
    }
}
