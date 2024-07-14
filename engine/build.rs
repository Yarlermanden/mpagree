fn main() -> Result<(), Box<dyn std::error::Error>> {
	tonic_build::configure()
		.include_file("mod.rs")
		.compile(&["../protos/mpagree/validator/service.proto", "../protos/mpagree/service/service.proto"], &["../protos"])?;
	Ok(())
}
