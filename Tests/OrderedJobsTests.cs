using System;
using NUnit.Framework;
using OrderedJobsKata;

namespace Tests
{
    [TestFixture]
    public class OrderedJobsTests
    {
        private JobMachine machine;

        [SetUp]
        public void Arrange()
        {
            machine = new JobMachine();
        }

        [TestCase("", "")]
        [TestCase("a => ", "a")]
        public void Order_SimpleString_ReturnsSimpleSequence(string input, string output)
        {
            var result = machine.Order(input);
            Assert.That(result, Is.EqualTo(output));
        }

        [Test]
        public void Order_MultipleJobs_ReturnsAllJobs()
        {
            var jobs = "a =>  \n" +
                       "b =>  \n" +
                       "c => ";
            var result = machine.Order(jobs);
            Assert.That(result, Is.EqualTo("a,b,c"));
        }

        [Test]
        public void Order_MultipleJobsSingleDependency_ReturnsCBeforeB()
        {
            var jobs = "a =>  \n" +
                       "b => c\n" +
                       "c => ";
            var result = machine.Order(jobs);
            Assert.That(result, Is.EqualTo("a,c,b"));
        }

        [TestCase("f", "c")]
        [TestCase("c", "b")]
        [TestCase("b", "e")]
        [TestCase("a", "d")]
        public void Order_MultipleJobsMultipleDependencies_ReturnsCorrectSequence(string first, string second)
        {
            var jobs = "a =>  \n" +
                       "b => c\n" +
                       "c => f\n" +
                       "d => a\n" +
                       "e => b\n" +
                       "f => ";
            var result = machine.Order(jobs);
            Assert.That(result.IndexOf(first) < result.IndexOf(second), 
                            Is.True, string.Format("{0} not before {1} in {2}", first, second, result));
        }

        [Test]
        public void Order_JobReferencesSameJob_ThrowsException()
        {
            var jobs = "a =>  \n" +
                       "b =>  \n" +
                       "c => c";

            TestDelegate tst = () => { machine.Order(jobs); };

            Assert.Throws<Exception>(tst);

        }

        [Test]
        public void Order_MultipleJobsCircularDependency_ThrowsException()
        {
            var jobs = "a =>  \n" +
                       "b => c\n" +
                       "c => f\n" +
                       "d => a\n" +
                       "e =>  \n" +
                       "f => b";

            TestDelegate tst = () => { machine.Order(jobs); };

            Assert.Throws<Exception>(tst);
        }

    }
}