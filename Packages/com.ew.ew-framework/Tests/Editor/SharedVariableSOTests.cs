using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using EW_Framework.Core.SharedVariables.DataTypeDrivenVariable;

namespace EW_Framework.Tests.Editor
{
    public class SharedVariableSOTests
    {
        private SharedIntSO _variable;
        private List<int> _received;

        [SetUp]
        public void SetUp()
        {
            _variable = ScriptableObject.CreateInstance<SharedIntSO>();
            _received = new List<int>();
            _variable.OnValueChanged += _received.Add;
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_variable);

        [Test]
        public void SettingNewValue_RaisesOnValueChangedWithThatValue()
        {
            _variable.Value = 5;

            Assert.AreEqual(5, _variable.Value);
            CollectionAssert.AreEqual(new[] { 5 }, _received);
        }

        [Test]
        public void SettingEqualValue_DoesNotRaiseOnValueChanged()
        {
            _variable.Value = 3;
            _variable.Value = 3;

            CollectionAssert.AreEqual(new[] { 3 }, _received);
        }

        [Test]
        public void LoadData_RestoresSavedValueAndRaisesOnValueChanged()
        {
            var source = ScriptableObject.CreateInstance<SharedIntSO>();
            try
            {
                source.Value = 7;
                string json = source.GetSaveData();

                _variable.LoadData(json);

                Assert.AreEqual(7, _variable.Value);
                CollectionAssert.AreEqual(new[] { 7 }, _received);
            }
            finally
            {
                Object.DestroyImmediate(source);
            }
        }
    }
}
