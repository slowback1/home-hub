# Deploying with GitHub Actions

This directory contains files for testing and deploying the project using GitHub Actions. These should be relatively plug-and-play, where all you would need to provide is the necessary secrets and environment variables.

## Usage

Move the `.yml` files in this directory to the `.github/workflows` directory in your project. You will need to provide the necessary secrets and environment variables in the GitHub repository settings.

### deploy-to-s3.yml

This workflow is used to deploy the project to an S3 bucket, and invalidate the associated cloudfront distribution. You will need to provide the following secrets:

- `AWS_ACCESS_KEY_ID` - The access key ID for the AWS account.
- `AWS_SECRET_ACCESS_KEY` - The secret access key for the AWS account.
- `AWS_REGION` - The region where the S3 bucket is located.
- `AWS_S3_BUCKET` - The name of the S3 bucket where the project will be deployed.
- `AWS_CLOUDFRONT_DISTRIBUTION_ID` - The ID of the CloudFront distribution that is associated with the S3 bucket.

### run-unit-tests.yml

This workflow is used to run the unit tests for the project. This workflow does not require any secrets or environment variables.
