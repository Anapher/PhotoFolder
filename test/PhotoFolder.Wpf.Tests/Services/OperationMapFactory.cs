using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Domain.Template;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Extensions;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.ViewModels.Models;
using Xunit;

namespace PhotoFolder.Wpf.Tests.Services
{
    public class OperationMapFactoryTests
    {
        [Fact]
        public void TestInvalidLocationOutside()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "AA" },
                {"2018/21.09/test2.jpg", "FF" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateInvalidLocationIssue("C:/photos/newPhoto.jpg", "EE", "2018/23.3/test.jpg");

            // act
            var result = OperationMapFactory.Create(issue.Yield(), false, indexedFiles);

            // assert
            var operation = Assert.Single(result);
            Assert.Equal(FileBaseChange.NewFile, operation.FileBaseChange);
        }

        [Fact]
        public void TestInvalidLocationInside()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "AA" },
                {"2018/21.09/test2.jpg", "FF" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateInvalidLocationIssue("2018/21.09/test.jpg", "AA", "2018/21.09/test5.jpg");

            // act
            var result = OperationMapFactory.Create(issue.Yield(), false, indexedFiles);

            // assert
            var operation = Assert.Single(result);
            Assert.Equal(FileBaseChange.NoChanges, operation.FileBaseChange);
        }

        [Fact]
        public void TestDuplicateFilesDeleteAll()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "AA" },
                {"2018/21.09/test2.jpg", "AA" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateDuplicateFileIssue(new[]
            {
                ("C:/photos/2018/21.09/test.jpg", "2018/21.09/test.jpg", "AA", true),
                ("C:/photos/2018/21.09/test2.jpg", "2018/21.09/test2.jpg", "AA", true)
            });

            // act
            var result = OperationMapFactory.Create(issue.Yield(), false, indexedFiles);

            // assert
            void ValidationAction(FileOperationInfo info)
            {
                Assert.Equal(FileBaseChange.FileDeleted, info.FileBaseChange);
            }

            Assert.Collection(result, ValidationAction, ValidationAction);
        }

        [Fact]
        public void TestDuplicateFilesDeleteOne()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "AA" },
                {"2018/21.09/test2.jpg", "AA" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateDuplicateFileIssue(new[]
            {
                ("C:/photos/2018/21.09/test.jpg", "2018/21.09/test.jpg", "AA", true),
                ("C:/photos/2018/21.09/test2.jpg", "2018/21.09/test2.jpg", "AA", false)
            });

            // act
            var result = OperationMapFactory.Create(issue.Yield(), false, indexedFiles);

            // assert
            var operation = Assert.Single(result);
            Assert.Equal(FileBaseChange.NoChanges, operation.FileBaseChange);
        }

        [Fact]
        public void TestDuplicateFilesDeleteOutside()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "FF" },
                {"2018/21.09/test2.jpg", "AA" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateDuplicateFileIssue(new[]
            {
                ("D:/DCM/asd.jpg", null, "AA", true),
                ("C:/photos/2018/21.09/test2.jpg", "2018/21.09/test2.jpg", "AA", false)
            });

            // act
            var result = OperationMapFactory.Create(issue.Yield(), true, indexedFiles);

            // assert
            var operation = Assert.Single(result);
            Assert.Equal(FileBaseChange.NoChanges, operation.FileBaseChange);
        }

        [Fact]
        public void Test2123()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "FF" },
                {"2018/21.09/test2.jpg", "AA" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateDuplicateFileIssue(new[]
            {
                ("D:/DCM/asd.jpg", null, "AA", true),
                ("C:/photos/2018/21.09/test2.jpg", "2018/21.09/test2.jpg", "AA", false)
            });

            // act
            var result = OperationMapFactory.Create(issue.Yield(), true, indexedFiles);

            // assert
            var operation = Assert.Single(result);
            Assert.Equal(FileBaseChange.NoChanges, operation.FileBaseChange);
        }

        [Fact]
        public void TestDuplicateFilesIgnoreOutside()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "FF" },
                {"2018/21.09/test2.jpg", "AA" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateDuplicateFileIssue(new[]
            {
                ("D:/DCM/asd.jpg", null, "AA", true),
                ("C:/photos/2018/21.09/test2.jpg", "2018/21.09/test2.jpg", "AA", false)
            });

            // act
            var result = OperationMapFactory.Create(issue.Yield(), false, indexedFiles);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void TestDuplicateFilesDeleteInsideAndKeepOutside()
        {
            // arrange
            var files = new Dictionary<string, string>
            {
                {"2018/21.09/test.jpg", "FF" },
                {"2018/21.09/test2.jpg", "AA" },
                {"2018/21.09/test3.jpg", "DD" },
            };

            var indexedFiles = CreateIndexedFiles(files);
            var issue = CreateDuplicateFileIssue(new[]
            {
                ("D:/DCM/asd.jpg", null, "AA", false),
                ("C:/photos/2018/21.09/test2.jpg", "2018/21.09/test2.jpg", "AA", true)
            });

            // act
            var result = OperationMapFactory.Create(issue.Yield(), false, indexedFiles);

            // assert
            var operation = Assert.Single(result);
            Assert.Equal(FileBaseChange.FileDeleted, operation.FileBaseChange);
        }

        private static InvalidLocationFileDecisionViewModel CreateInvalidLocationIssue(string path, string hash, string targetPath,
            string? relativeFilename = null)
        {
            var issue = new InvalidFileLocationIssue(CreateFileInformation(path, hash, relativeFilename), TemplateString.Parse("test"),
                new[] {new FilenameSuggestion(Path.GetDirectoryName(targetPath), targetPath),}, "");

            return new InvalidLocationFileDecisionViewModel(issue);
        }

        private static DuplicateFileDecisionViewModel CreateDuplicateFileIssue(IReadOnlyList<(string path, string? relativeFilename, string hash, bool delete)> files)
        {
            var fileInformation = files.Select(x => CreateFileInformation(x.path, x.hash, x.relativeFilename)).ToList();
            var issue = new DuplicateFilesIssue(fileInformation.First(), fileInformation.Skip(1));
            return new DuplicateFileDecisionViewModel(issue,
                fileInformation.Select(x => new Checkable<FileInformation>(x, !files.First(y => y.path == x.Filename).delete)).ToList());
        }

        private static FileInformation CreateFileInformation(string path, string hash, string? relativeFilename)
        {
            return new FileInformation(path, default, default, Hash.Parse(hash), 3242, default, default, relativeFilename);
        }

        private static IReadOnlyList<IndexedFile> CreateIndexedFiles(IReadOnlyDictionary<string, string> files)
        {
            return files.GroupBy(x => x.Value).Select(x =>
            {
                var indexedFile = new IndexedFile(Hash.Parse(x.Key), 72384, default, default);
                foreach (var path in x.Select(x => x.Key))
                    indexedFile.AddLocation(new FileLocation(path, x.Key, default, default));

                return indexedFile;
            }).ToList();
        }
    }
}
