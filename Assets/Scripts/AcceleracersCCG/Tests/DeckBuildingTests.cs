using System.Collections.Generic;
using NUnit.Framework;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class DeckBuildingTests
    {
        [Test]
        public void ValidDeck_NoErrors()
        {
            var cards = new List<CardData>
            {
                TestHelpers.MakeVehicle("v1"),
                TestHelpers.MakeVehicle("v2"),
                TestHelpers.MakeMod("m1"),
                TestHelpers.MakeMod("m1"),
                TestHelpers.MakeMod("m1"),
                TestHelpers.MakeShift("s1"),
                TestHelpers.MakeShift("s1"),
                TestHelpers.MakeHazard("h1"),
            };

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void OverMaxDeckSize_Error()
        {
            var cards = new List<CardData>();
            for (int i = 0; i < 81; i++)
                cards.Add(TestHelpers.MakeMod($"m{i}"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsNotEmpty(errors);
            Assert.IsTrue(errors[0].Contains("81"));
        }

        [Test]
        public void DuplicateVehicle_Error()
        {
            var cards = new List<CardData>
            {
                TestHelpers.MakeVehicle("v1"),
                TestHelpers.MakeVehicle("v1"),
            };

            var errors = DeckBuildingRules.Validate(cards);
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void DuplicateAcceleCharger_Error()
        {
            var cards = new List<CardData>
            {
                TestHelpers.MakeAcceleCharger("ac1"),
                TestHelpers.MakeAcceleCharger("ac1"),
            };

            var errors = DeckBuildingRules.Validate(cards);
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void FourCopiesOfMod_Error()
        {
            var cards = new List<CardData>();
            for (int i = 0; i < 4; i++)
                cards.Add(TestHelpers.MakeMod("m1"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void ThreeCopiesOfMod_Valid()
        {
            var cards = new List<CardData>();
            for (int i = 0; i < 3; i++)
                cards.Add(TestHelpers.MakeMod("m1"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void RealmInDeck_Error()
        {
            var cards = new List<CardData>
            {
                TestHelpers.MakeRealm("r1"),
            };

            var errors = DeckBuildingRules.Validate(cards);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Racing Realm"));
        }

        [Test]
        public void ExactlyMaxDeckSize_Valid()
        {
            var cards = new List<CardData>();
            for (int i = 0; i < 80; i++)
                cards.Add(TestHelpers.MakeMod($"m{i}"));

            var errors = DeckBuildingRules.Validate(cards);
            Assert.IsEmpty(errors);
        }
    }
}
