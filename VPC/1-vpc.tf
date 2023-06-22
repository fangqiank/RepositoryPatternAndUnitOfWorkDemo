provider "aws" {
	region = "eu-west-1"
}

resource "aws_vpc" "custom_vpc" {
	cidr_block = "10.0.0.0/16" #ip range available inside the vpc
	instance_tenancy = "default"

	tags ={
		"Name" = "custom_vpc"
	}
}