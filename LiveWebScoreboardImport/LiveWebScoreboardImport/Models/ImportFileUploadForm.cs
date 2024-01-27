namespace LiveWebScoreboardImport.Models {

	#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public class ImportFileUploadForm {
		private readonly String curModuleName = "ImportFileUploadForm: ";

		public string PublishFilename { get; set; }

		public string PublishFilenameBase { get; set; }

		public IFormFile PublishFile { get; set; }

	}
}
