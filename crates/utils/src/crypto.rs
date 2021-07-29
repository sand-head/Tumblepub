use anyhow::Result;
use openssl::{pkey::PKey, rsa::Rsa};

pub struct KeyPair {
  pub private_key: Vec<u8>,
  pub public_key: Vec<u8>,
}
impl KeyPair {
  pub fn generate() -> Result<Self> {
    let rsa_pair = Rsa::generate(2048)?;
    let public_key = PKey::from_rsa(rsa_pair)?;

    Ok(KeyPair {
      private_key: public_key.private_key_to_pem_pkcs8()?,
      public_key: public_key.public_key_to_pem()?,
    })
  }
}
