mod pb {
	tonic::include_proto!("mod");
}

pub mod validator {
	pub use crate::pb::validator::*;
}

pub mod server {
	pub use crate::pb::service::*;
}

pub mod grpc;

pub mod model;
