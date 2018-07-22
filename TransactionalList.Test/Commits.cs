using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TransactionalList.Test
{
    [TestClass]
    public class Commits
    {
        [TestMethod]
        public void AddOne()
        {
            var list = new SwapTransactionalList<TransactionalItem>();
            var item1 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item1" };
            list.AddOrUpdate(item1);
            Assert.IsTrue(list.CurrentList.CompareList(item1));
            Assert.AreEqual(item1, list.CurrentList[0]);
            list.Commit();
            Assert.IsTrue(list.BaseList.CompareList(item1));
        }

        [TestMethod]
        public void AddTwo()
        {
            var list = new SwapTransactionalList<TransactionalItem>();
            var item1 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item1" };
            var item2 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item2" };
            list.AddOrUpdate(item1);
            list.AddOrUpdate(item2);
            Assert.IsTrue(list.CurrentList.CompareList(item1, item2));
            list.Commit();
            Assert.IsTrue(list.BaseList.CompareList(item1, item2));
        }

        [TestMethod]
        public void UpdateOne()
        {
            var list = new SwapTransactionalList<TransactionalItem>();
            var item1 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item1" };
            var item2 = new TransactionalItem { ID = item1.ID, Name = "item2" };
            list.AddOrUpdate(item1);
            list.AddOrUpdate(item2);
            Assert.IsTrue(list.CurrentList.CompareList(item2));
            list.Commit();
            Assert.IsTrue(list.BaseList.CompareList(item2));
        }

        [TestMethod]
        public void DeleteOne()
        {
            var list = new SwapTransactionalList<TransactionalItem>();
            var item1 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item1" };
            var item2 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item2" };
            list.AddOrUpdate(item1);
            Assert.IsTrue(list.CurrentList.CompareList(item1));
            list.AddOrUpdate(item2);
            Assert.IsTrue(list.CurrentList.CompareList(item1, item2));
            list.Commit();
            Assert.IsTrue(list.BaseList.CompareList(item1, item2));
            list.Remove(item2);
            Assert.IsTrue(list.CurrentList.CompareList(item1));
            list.Commit();
            Assert.IsTrue(list.CurrentList.CompareList(item1));
        }

        [TestMethod]
        public void BaseList()
        {
            var item1 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item1" };
            var item2 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item2" };
            var item3 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item3" };
            var item4 = new TransactionalItem { ID = Guid.NewGuid(), Name = "item4" };
            var list = new SwapTransactionalList<TransactionalItem>(new List<TransactionalItem> { item1, item2});

            Assert.IsTrue(list.CurrentList.CompareList(item1, item2));
            Assert.IsTrue(list.BaseList.CompareList(item1, item2));
            list.AddOrUpdate(item3);
            Assert.IsTrue(list.CurrentList.CompareList(item1, item2, item3));
            list.AddOrUpdate(item4);
            Assert.IsTrue(list.CurrentList.CompareList(item1, item2, item3, item4));
            list.Commit();
            Assert.IsTrue(list.BaseList.CompareList(item1, item2, item3, item4));
        }
    }
}
