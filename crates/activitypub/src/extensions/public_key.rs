use activitystreams::{unparsed::UnparsedMutExt, url::Url};
use activitystreams_ext::UnparsedExtension;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct PublicKey {
  pub public_key: PublicKeyInner,
}

#[derive(Clone, Debug, Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct PublicKeyInner {
  pub id: Url,
  pub owner: Url,
  pub public_key_pem: String,
}

impl<U> UnparsedExtension<U> for PublicKey
where
  U: UnparsedMutExt,
{
  type Error = serde_json::Error;

  fn try_from_unparsed(unparsed_mut: &mut U) -> Result<Self, Self::Error>
  where
    Self: Sized,
  {
    Ok(PublicKey {
      public_key: unparsed_mut.remove("public_key")?,
    })
  }

  fn try_into_unparsed(self, unparsed_mut: &mut U) -> Result<(), Self::Error> {
    unparsed_mut.insert("public_key", self.public_key)?;
    Ok(())
  }
}
