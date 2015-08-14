using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PfxBruteForce.UI.Controllers;
using PfxBruteForce.UI.Controllers.ViewModels;
using PfxBruteForce.UI.Generators;
using PfxBruteForce.UI.Utils;

namespace PfxBruteForce.UI.Tests.Controllers
{
    [TestFixture]
    public class MainControllerTest
    {
        [Test]
        public void Constructing_DoesNotThrow()
        {
            var target = new MainController(
                Mock.Of<CompositeGenerator>(),
                Mock.Of<PasswordTester>(),
                Mock.Of<SpeedCalculator>()
            );
        }

        [Test]
        public void Init_IsNotRunningByDefault()
        {
            var target = new MainController(
                Mock.Of<CompositeGenerator>(),
                Mock.Of<PasswordTester>(),
                Mock.Of<SpeedCalculator>()
            );

            var data = target.Init();

            Assert.IsFalse(data.Running);
        }

        [Test]
        public async void Start_FooListWithFooTester_Succeeds()
        {
            var target = new MainController(
                new FooGenerator(),
                new FooTester(),
                Mock.Of<SpeedCalculator>()
            );
            var data = target.Init();

            await target.Start(new MainFormStartParameter());

            Assert.IsTrue(data.Found);
            Assert.AreEqual("foo", data.FoundPassword);
        }

        [Test]
        public async void Start_BarListWithFooTester_Fails()
        {
            var target = new MainController(
                new BarGenerator(),
                new FooTester(),
                Mock.Of<SpeedCalculator>()
            );
            var data = target.Init();

            await target.Start(new MainFormStartParameter());

            Assert.IsFalse(data.Found);
            Assert.IsEmpty(data.FoundPassword);
        }

        public class FooTester : PasswordTester
        {
            public override void Init(string targetPath)
            {
            }

            public override async Task<string> Test(ICollection<string> passwords)
            {
                return passwords.FirstOrDefault(p => p == "foo");
            }
        }

        public class FooGenerator : CompositeGenerator
        {
            private bool firstTime = true;

            protected virtual string UniquePassword { get { return "foo"; } }

            public override CompositeGenerator Init(int minLength, int maxLength, string dictionaryUrl)
            {
                return this;
            }

            public override async Task<ICollection<string>> GetBucket(int size)
            {
                if (firstTime)
                {
                    firstTime = false;
                    return new[] { "apuo", UniquePassword, "sdpofispdfoi" };

                }
                else
                    return new string[0];
            }
        }

        public class BarGenerator : FooGenerator
        {
            protected override string UniquePassword
            {
                get { return "bar"; }
            }
        }
    }
}