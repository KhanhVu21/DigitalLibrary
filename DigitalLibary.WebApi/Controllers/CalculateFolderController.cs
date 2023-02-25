using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculateFolderController: Controller
    {
        #region Variables
        private readonly AppSettingModel _appSettingModel;
        private readonly ICalculateFolder _calculateFolder;
        #endregion

        #region Contructor
        public CalculateFolderController(
        IOptionsMonitor<AppSettingModel> optionsMonitor, ICalculateFolder calculateFolder)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _calculateFolder = calculateFolder;
        }
        #endregion

        #region METHOD
        // GET: api/CalculateFolder/CalculateFolder
        [HttpGet]
        [Route("CalculateFolder")]
        public IActionResult CalculateFolder()
        {
            try
            {
                // get capacity from database
                var result = _calculateFolder.calculateDatabases();

                // Get the directory information using directoryInfo() method
                DirectoryInfo folder = new DirectoryInfo(_appSettingModel.Root);

                // Calling a folderSize() method
                long totalFolderSize = folderSize(folder);

                return Ok(new
                {
                    rootFolderBytes = totalFolderSize,
                    rootFolderMB = totalFolderSize * 0.00000095367432,
                    maxSizeGB = 7,
                    database = result
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Function to calculate the size of the folder
        static long folderSize(DirectoryInfo folder)
        {
            long totalSizeOfDir = 0;

            // Get all files into the directory
            FileInfo[] allFiles = folder.GetFiles();

            // Loop through every file and get size of it
            foreach (FileInfo file in allFiles)
            {
                totalSizeOfDir += file.Length;
            }

            // Find all subdirectories
            DirectoryInfo[] subFolders = folder.GetDirectories();

            // Loop through every subdirectory and get size of each
            foreach (DirectoryInfo dir in subFolders)
            {
                totalSizeOfDir += folderSize(dir);
            }

            // Return the total size of folder
            return totalSizeOfDir;
        }
        #endregion
    }
}
