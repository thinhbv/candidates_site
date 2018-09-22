using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Configuration.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment.Descriptor
{
    public class ShellDescriptorManager : IShellDescriptorManager
    {
        private readonly IRepository<ShellDescriptorRecord, Guid> shellDescriptorRepository;
        private readonly IShellDescriptorManagerEventHandler events;
        private readonly ShellSettings shellSettings;

        public ShellDescriptorManager(
            IRepository<ShellDescriptorRecord, Guid> shellDescriptorRepository,
            IShellDescriptorManagerEventHandler events,
            ShellSettings shellSettings)
        {
            this.shellDescriptorRepository = shellDescriptorRepository;
            this.events = events;
            this.shellSettings = shellSettings;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ShellDescriptor GetShellDescriptor()
        {
            ShellDescriptorRecord shellDescriptorRecord = GetDescriptorRecord();
            if (shellDescriptorRecord == null)
            {
                return null;
            }
            return GetShellDescriptorFromRecord(shellDescriptorRecord);
        }

        private static ShellDescriptor GetShellDescriptorFromRecord(ShellDescriptorRecord shellDescriptorRecord)
        {
            var descriptor = new ShellDescriptor { SerialNumber = shellDescriptorRecord.SerialNumber };
            var descriptorFeatures = new List<ShellFeature>();

            if (!string.IsNullOrWhiteSpace(shellDescriptorRecord.Features))
            {
                var split = shellDescriptorRecord.Features.Split(';');
                descriptorFeatures.AddRange(split.Select(str => new ShellFeature { Name = str }));
            }
            descriptor.Features = descriptorFeatures;

            return descriptor;
        }

        private ShellDescriptorRecord GetDescriptorRecord()
        {
            return shellDescriptorRepository.Table.FirstOrDefault();
        }

        public void UpdateShellDescriptor(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, bool throwEvent = true)
        {
            var shellDescriptorRecord = GetDescriptorRecord();
            var serialNumber = shellDescriptorRecord == null ? 0 : shellDescriptorRecord.SerialNumber;

            //2013-12-18: Hack
            //bool isFirstRun = (serialNumber == 0 && priorSerialNumber == 1) || (serialNumber == 1 && priorSerialNumber == 0);

            //if (!isFirstRun)
            //{
            //    if (priorSerialNumber != serialNumber)
            //    {
            //        throw new InvalidOperationException(T("Invalid serial number for shell descriptor").ToString());
            //    }
            //}

            if (shellDescriptorRecord == null)
            {
                shellDescriptorRecord = new ShellDescriptorRecord
                {
                    Id = Guid.NewGuid(),
                    SerialNumber = 1,
                    Features = string.Join(";", enabledFeatures.Select(x => x.Name))
                };

                shellDescriptorRepository.Insert(shellDescriptorRecord);
            }
            else
            {
                shellDescriptorRecord.SerialNumber++;
                shellDescriptorRecord.Features = string.Join(";", enabledFeatures.Select(x => x.Name));
                shellDescriptorRepository.Update(shellDescriptorRecord);
            }

            if (throwEvent)
            {
                events.Changed(GetShellDescriptorFromRecord(shellDescriptorRecord), shellSettings.Name);
            }
        }
    }
}