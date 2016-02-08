﻿using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Languages.Core.Test.Tokens;
using Microsoft.R.Core.Tokens;
using Microsoft.UnitTests.Core.XUnit;
using Xunit;

namespace Microsoft.R.Core.Test.Tokens {
    [ExcludeFromCodeCoverage]
    public class TokenizeFloatsTest : TokenizeTestBase<RToken, RTokenType> {
        private readonly CoreTestFilesFixture _files;

        public TokenizeFloatsTest(CoreTestFilesFixture files) {
            _files = files;
        }

        [CompositeTest]
        [InlineData("+1 ", 0, 2)]
        [InlineData("-.0", 0, 3)]
        [InlineData("0.e1", 0, 4)]
        [InlineData(".0e-2", 0, 5)]
        [Category.R.Tokenizer]
        public void TokenizeFloats(string text, int start, int length) {
            var tokens = Tokenize(text, new RTokenizer());

            tokens.Should().ContainSingle()
                .Which.Should().HaveType(RTokenType.Number)
                .And.StartAt(start)
                .And.HaveLength(length);

        }

        [Test]
        [Category.R.Tokenizer]
        public void TokenizeFloats5() {
            var tokens = Tokenize("-0.e", new RTokenizer());
            tokens.Should().BeEmpty();
        }

        [Test]
        [Category.R.Tokenizer]
        public void TokenizeFloats6() {
            var tokens = Tokenize("-12.%foo%-.1e", new RTokenizer());

            tokens.Should().HaveCount(2);

            tokens[0].Should().HaveType(RTokenType.Number)
                .And.StartAt(0)
                .And.HaveLength(4);

            tokens[1].Should().HaveType(RTokenType.Operator)
                .And.StartAt(4)
                .And.HaveLength(5);
        }

        [Test]
        [Category.R.Tokenizer]
        public void TokenizeFloats7() {
            var tokens = Tokenize(".1", new RTokenizer());

            tokens.Should().ContainSingle()
                .Which.Should().HaveType(RTokenType.Number)
                .And.StartAt(0)
                .And.HaveLength(2);
        }

        [Test]
        [Category.R.Tokenizer]
        public void TokenizeFloats8() {
            var tokens = Tokenize("1..1", new RTokenizer());

            tokens.Should().HaveCount(2);

            tokens[0].Should().HaveType(RTokenType.Number)
                .And.StartAt(0)
                .And.HaveLength(2);

            tokens[1].Should().HaveType(RTokenType.Number)
                .And.StartAt(2)
                .And.HaveLength(2);
        }

        [Test]
        [Category.R.Tokenizer]
        public void TokenizeFloats9() {
            var tokens = Tokenize("1e", new RTokenizer());
            tokens.Should().BeEmpty();
        }

        [Test]
        [Category.R.Tokenizer]
        public void TokenizeFile_FloatsFile() {
            TokenizeFiles.TokenizeFile(_files, @"Tokenization\Floats.r");
        }
    }
}
