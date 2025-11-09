namespace OpenIso8583Net.Emv
{
    /// <summary>
    /// EMV Tag
    /// </summary>
    public enum Tag
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Object Identifier (OID)
        /// </summary>
        object_identifier = 0x06,

        /// <summary>
        /// Country code and national data
        /// </summary>
        country_code = 0x41,

        /// <summary>
        /// Issuer Identification Number (IIN) - The number that identifies the major industry and the card issuer 
        /// and that forms the first part of the Primary Account Number (PAN)
        /// </summary>
        issuer_id_num = 0x42,

        /// <summary>
        /// Card service data
        /// </summary>
        card_service_data = 0x43,

        /// <summary>
        /// Initial access data
        /// </summary>
        initial_access_data = 0x44,

        /// <summary>
        /// Card issuer's data
        /// </summary>
        card_issuers_data = 0x45,

        /// <summary>
        /// Pre-issuing data
        /// </summary>
        pre_issuing_data = 0x46,

        /// <summary>
        /// Card capabilities
        /// </summary>
        card_capabilities = 0x47,

        /// <summary>
        /// Status information
        /// </summary>
        status_info = 0x48,

        /// <summary>
        /// Extended header list
        /// </summary>
        header_list_extended = 0x4D,

        /// <summary>
        /// Application Identifier (ADF Name) 
        /// </summary>
        /// <remarks>
        /// The ADF Name identifies the application as described in [ISO 7816-5]. 
        /// The AID is made up of the Registered Application Provider Identifier 
        /// (RID) and the Proprietary Identifier Extension (PIX).
        /// </remarks>
        aid = 0x4F,

        /// <summary>
        /// Application Label.
        /// </summary>
        /// <remarks>
        /// Mnemonic associated with AID according to [ISO 7816-5]. Used in application selection. Application Label is optional in the 
        /// File Control Information (FCI) of an Application Definition File (ADF) and optional in an ADF directory entry.
        /// </remarks>
        appl_label = 0x50,

        /// <summary>
        /// Path
        /// </summary>
        /// <remarks>
        /// A path may reference any file. It is a concatenation of file identifiers. The path begins with the identifier of a DF 
        /// (the MF for an absolute path or the current DF for a relative path) and ends with the identifier of the file itself.
        /// </remarks>
        path = 0x51,

        /// <summary>
        /// Command to perform
        /// </summary>
        cmd_to_perform = 0x52,

        /// <summary>
        /// Discretionary data, discretionary template
        /// </summary>
        discretionary_data = 0x53,

        /// <summary>
        /// Track 1 Data
        /// </summary>
        /// <remarks>
        /// Track 1 Data contains the data objects of the track 1 according to [ISO/IEC 7813] Structure B, 
        /// excluding start sentinel, end sentinel and LRC. The Track 1 Data may be present in the file 
        /// read using the READ RECORD command during a mag-stripe mode transaction.
        /// </remarks>
        track1_data = 0x56,



        /// <summary>
        /// Track 2 Equivalent Data
        /// </summary>
        /// <remarks>
        /// Contains the data elements of track 2 according to ISO/IEC 7813, excluding start sentinel, 
        /// end sentinel, and Longitudinal Redundancy Check (LRC), as follows:
        /// * Primary Account Number
        /// * Field Separator (Hex 'D')
        /// * Expiration Date (YYMM)
        /// * Service Code
        /// * Discretionary Data (defined by individual payment systems)
        /// * Pad with one Hex 'F' if needed to ensure whole bytes
        /// </remarks>
        track2_eq_data = 0x57,

        /// <summary>
        /// Track 3 Equivalent Data
        /// </summary>
        track3_eq_data = 0x58,

        /// <summary>
        /// Card expiration date
        /// </summary>
        card_exp_date = 0x59,

        /// <summary>
        /// Application Primary Account Number (PAN)
        /// </summary>
        appl_pan = 0x5a,

        /// <summary>
        /// Name of an individual
        /// </summary>
        name = 0x5B,

        /// <summary>
        /// Tag list
        /// </summary>
        tag_list = 0x5C,

        /// <summary>
        /// Header list
        /// </summary>
        header_list = 0x5D,

        proprietary_login_data = 0x5E,

        /// <summary>
        /// Cardholder Name - Indicates cardholder name according to ISO 7813
        /// </summary>
        cardholder_name = 0x5f20,

        /// <summary>
        /// Track 1, identical to the data coded
        /// </summary>
        track1 = 0x5F21,

        /// <summary>
        /// Track 2, identical to the data coded
        /// </summary>
        track2 = 0x5F22,

        /// <summary>
        /// Track 3, identical to the data coded
        /// </summary>
        track3 = 0x5F23,

        /// <summary>
        /// Application Expiration Date
        /// </summary>
        /// <remarks>
        /// Date after which application expires. The date is expressed in the YYMMDD format. 
        /// For MasterCard applications, if the value of YY ranges from '00' to '49' the date 
        /// reads 20YYMMDD. If the value of YY ranges from '50' to '99' the date reads 19YYMMDD.
        /// </remarks>
        app_expiry_date = 0x5F24,

        /// <summary>
        /// Application Effective Date
        /// </summary>
        /// <remarks>
        /// Date from which the application may be used. The date is expressed in the YYMMDD format. 
        /// For MasterCard branded applications if the value of YY ranges from '00' to '49' the date 
        /// reads 20YYMMDD. If the value of YY ranges from '50' to '99', the date reads 19YYMMDD.
        /// </remarks>
        app_effect_date = 0x5f25,

        /// <summary>
        /// Date, Card Effective
        /// </summary>
        card_effective_date = 0x5F26,

        /// <summary>
        /// Interchange control
        /// </summary>
        interchage_control = 0x5F27,

        /// <summary>
        /// The issuer_country_code.
        /// </summary>
        issuer_country_code = 0x5f28,

        /// <summary>
        /// Interchange profile	
        /// </summary>
        interchange_profile = 0x5F29,

        /// <summary>
        /// Transaction Currency Code
        /// </summary>
        /// <remarks>
        /// Indicates the currency code of the transaction according to [ISO 4217]. 
        /// The implied exponent is indicated by the minor unit of currency associated
        /// with the Transaction Currency Code in [ISO 4217].
        /// </remarks>
        txn_curcy_code = 0x5f2a,

        /// <summary>
        /// Date of birth
        /// </summary>
        date_of_birth = 0x5F2B,

        /// <summary>
        /// Language Preference
        /// </summary>
        /// <remarks>
        /// 1-4 languages stored in order of preference, each represented by 2 alphabetical characters according to ISO 639
        /// Note: EMVCo strongly recommends that cards be personalised with data element '5F2D' coded in lowercase, 
        /// but that terminals accept the data element whether it is coded in upper or lower case.
        /// </remarks>
        lang_preference = 0x5f2d,

        /// <summary>
        /// Cardholder biometric data	
        /// </summary>
        card_holder_biometric_data = 0x5F2E,

        /// <summary>
        /// PIN usage policy
        /// </summary>
        pin_usage_policy = 0x5F2F,

        /// <summary>
        /// Service Code
        /// </summary>
        /// <remarks>
        /// Service code as defined in ISO/IEC 7813 for Track 1 and Track 2
        /// </remarks>
        service_code = 0x5f30,

        /// <summary>
        /// Transaction counter
        /// </summary>
        txn_counter = 0x5F32,

        /// <summary>
        /// Date, Transaction
        /// </summary>
        txn_date = 0x5F33,

        /// <summary>
        /// Application Primary Account Number (PAN) Sequence Number (PSN)
        /// </summary>
        /// <remarks>
        /// Identifies and differentiates cards with the same Application PAN
        /// </remarks>
        pan_seqnum = 0x5f34,

        /// <summary>
        /// Sex (ISO 5218)
        /// </summary>
        /// <remarks>
        /// Representation of human sexes through a language-neutral single-digit code 
        ///     0 = not known, 
        ///     1 = male, 
        ///     2 = female, 
        ///     9 = not applicable
        /// </remarks>
        sex = 0x5F35,

        /// <summary>
        /// Transaction Currency Exponent
        /// </summary>
        /// <remarks>
        /// Identifies the decimal point position from the right of the transaction amount accordin to ISO 4217
        /// </remarks>
        trans_curr_exp = 0x5f36,

        /// <summary>
        /// Static internal authentication (one-step)	
        /// </summary>
        static_int_auth_1step = 0x5F37,

        /// <summary>
        /// Static internal authentication - first associated data
        /// </summary>
        static_int_auth_first = 0x5F38,

        /// <summary>
        /// Static internal authentication - second associated data
        /// </summary>
        static_int_auth_second = 0x5F39,

        /// <summary>
        /// Dynamic internal authentication
        /// </summary>
        dynam_int_auth = 0x5F3A,

        /// <summary>
        /// Dynamic external authentication
        /// </summary>
        dynam_ext_auth = 0x5F3B,

        /// <summary>
        /// Dynamic mutual authentication
        /// </summary>
        dynam_mutual_auth = 0x5F3C,

        /// <summary>
        /// Transaction Reference Currency Exponent
        /// </summary>
        /// <remarks>
        /// Identifies the decimal point position from the right of the terminal common currency
        /// </remarks>
        txn_ref_currency_exp = 0x5F3D,

        /// <summary>
        /// Element list
        /// </summary>
        element_list = 0x5F41,

        /// <summary>
        /// Address
        /// </summary>
        address = 0x5F42,

        /// <summary>
        /// Cardholder handwritten signature image
        /// </summary>
        cardholder_signature_image = 0x5F43,

        /// <summary>
        /// Application image
        /// </summary>
        application_image = 0x5F44,

        /// <summary>
        /// Display message
        /// </summary>
        display_message = 0x5F45,

        /// <summary>
        /// Timer
        /// </summary>
        timer = 0x5F46,

        /// <summary>
        /// Message reference
        /// </summary>
        message_reference = 0x5F47,

        /// <summary>
        /// Cardholder private key
        /// </summary>
        cardholder_private_key = 0x5F48,

        /// <summary>
        /// Cardholder public key	
        /// </summary>
        cardholder_public_key = 0x5F40,

        /// <summary>
        /// Public key of certification authority
        /// </summary>
        public_key_cert_authority = 0x5F4A,

        /// <summary>
        /// Certificate holder authorization
        /// </summary>
        certificate_holder_authorization = 0x5F4C,

        /// <summary>
        /// Integrated circuit manufacturer identifier
        /// </summary>
        ic_manufacturer_id = 0x5F4D,

        /// <summary>
        /// Certificate content
        /// </summary>
        certificate_content = 0x5F4E,

        /// <summary>
        /// The issuer_url.
        /// </summary>
        issuer_url = 0x5f50,

        /// <summary>
        /// International Bank Account Number (IBAN)
        /// </summary>
        /// <remarks>
        /// Uniquely identifies the account of a customer at a financial institution as defined in ISO 13616.
        /// </remarks>
        int_bank_acc_no = 0x5f53,

        /// <summary>
        /// Bank Identifier Code (BIC)
        /// </summary>
        /// <remarks>
        /// Uniquely identifies a bank as defined in ISO 9362.
        /// </remarks>
        bank_id_code = 0x5f54,

        /// <summary>
        /// The issuer_country_code 2.
        /// </summary>
        issuer_country_code2 = 0x5f55,

        /// <summary>
        /// The issuer_country_code 3.
        /// </summary>
        issuer_country_code3 = 0x5f56,

        /// <summary>
        /// The account_type.
        /// </summary>
        account_type = 0x5f57,

        /// <summary>
        /// Template, Dynamic Authentication
        /// </summary>
        template_dynam_auth = 0x60,

        /// <summary>
        /// Application Template
        /// </summary>
        /// <remarks>
        /// Template containing one or more data objects relevant to an application directory entry according to [ISO 7816-5].
        /// </remarks>
        appl_templ_61 = 0x61,

        /// <summary>
        /// File Control Parameters (FCP) Template
        /// </summary>
        /// <remarks>
        /// Identifies the FCP template according to ISO/IEC 7816-4
        /// </remarks>
        file_ctrl_param_template = 0x62,

        /// <summary>
        /// File Control Information (FCI) Template
        /// </summary>
        /// <remarks>
        /// Identifies the FCI template according to ISO/IEC 7816-4
        /// </remarks>
        fci_templ_6f = 0x6f,

        /// <summary>
        /// Issuer Script Template 1
        /// </summary>
        /// <remarks>
        /// Contains proprietary issuer data for transmission to the ICC before the second GENERATE AC command
        /// </remarks>
        issuer_scrpt_templ_71 = 0x71,

        /// <summary>
        /// Issuer Script Command
        /// </summary>
        //issuer_script_cmd = 0x7186,

        /// <summary>
        /// Issuer Script Identifier
        /// </summary>
        //issuer_script_id = 0x719F18,

        /// <summary>
        /// The isuer_scrpt_templ_72.
        /// </summary>
        isuer_scrpt_templ_72 = 0x72,


        /// <summary>
        /// Directory Discretionary Template
        /// </summary>
        /// <remarks>
        /// Issuer discretionary part of the directory according to ISO/IEC 7816-5
        /// </remarks>
        dir_discr_templ_73 = 0x73,

        /// <summary>
        /// Response Message Template Format 2
        /// </summary>
        /// <remarks>
        /// Contains the data objects (with tags and lengths) returned by the ICC in response to a command
        /// </remarks>
        respmsg_fmt1_templ_77 = 0x77,

        /// <summary>
        /// Response Message Template Format 1
        /// </summary>
        /// <remarks>
        /// Contains the data objects (without tags and lengths) returned by the ICC in response to a command
        /// </remarks>
        respmsg_fmt2_templ_80 = 0x80,

        /// <summary>
        /// Amount, Authorised
        /// </summary>
        /// <remarks>
        /// Authorised amount of the transaction (excluding adjustments)	
        /// </remarks>
        amount_auth = 0x81,

        /// <summary>
        /// Application Interchange Profile (AIP)
        /// </summary>
        /// <remarks>
        /// Indicates the capabilities of the card to support specific functions in the application. 
        /// Kernel 3 shall not act on AIP bit settings that are not supported for Kernel 3 or that 
        /// are Reserved for Future Use (RFU).
        /// </remarks>
        appl_intchg_profile = 0x82,

        /// <summary>
        /// Command Template
        /// </summary>
        /// <remarks>
        /// Identifies the data field of a command message
        /// </remarks>
        cmd_template = 0x83,

        /// <summary>
        /// Dedicated File (DF) Name
        /// </summary>
        /// <remarks>
        /// Identifies the name of the DF as described in ISO/IEC 7816-4
        /// </remarks>
        df_name = 0x84,

        /// <summary>
        /// Issuer Script Command
        /// </summary>
        /// <remarks>
        /// Contains a command for transmission to the ICC
        /// </remarks>
        issuer_script_cmd = 0x86,

        /// <summary>
        /// Application Priority Indicator
        /// </summary>
        /// <remarks>
        /// Indicates the priority of a given application or group of applications in a directory
        /// </remarks>
        appl_priority_ind = 0x87,

        /// <summary>
        /// Short File Identifier (SFI)
        /// </summary>
        /// <remarks>
        /// Identifies the AEF referenced in commands related to a given ADF or DDF. 
        /// It is a binary data object having a value in the range 1 to 30 and with 
        /// the three high order bits set to zero.
        /// </remarks>
        short_file_id = 0x88,

        /// <summary>
        /// Authorisation Code
        /// </summary>
        /// <remarks>
        /// Non-zero value generated by the Authorisation Systems for an approved transaction.
        /// </remarks>
        auth_code = 0x89,

        /// <summary>
        /// Authorisation Response Code (ARC)
        /// </summary>
        /// <remarks>
        /// Data element generated by the Issuer Host System or the Reader indicating the disposition of the transaction.
        /// </remarks>
        auth_resp_code = 0x8a,

        /// <summary>
        /// Card Risk Management Data Object List 1 (CDOL1)
        /// </summary>
        /// <remarks>
        /// List of data objects (tag and length) to be passed to the ICC in the first GENERATE AC command
        /// </remarks>
        cdol1 = 0x8c,

        /// <summary>
        /// Card Risk Management Data Object List 2 (CDOL2)
        /// </summary>
        /// <remarks>
        /// List of data objects (tag and length) to be passed to the ICC in the second GENERATE AC command
        /// </remarks>
        cdol2 = 0x8d,

        /// <summary>
        /// Cardholder Verification Method (CVM) List
        /// </summary>
        /// <remarks>
        /// Identifies a prioritized list of methods of verification of the cardholder supported by the card application.
        /// </remarks>
        card_verify_method_list = 0x8e,

        /// <summary>
        /// Certification Authority Public Key Index (PKI)
        /// </summary>
        /// <remarks>
        /// Identifies the Certificate Authority's public key in conjunction with the RID for use in offline data authentication.
        /// </remarks>
        auth_pubkey_index = 0x8f,

        /// <summary>
        /// Issuer Public Key Certificate
        /// </summary>
        /// <remarks>
        /// Issuer's public key certified by a certificate authority for use in offline data authentication.
        /// </remarks>
        iss_pubkey_cert = 0x90,

        /// <summary>
        /// Issuer Authentication Data
        /// </summary>
        /// <remarks>
        /// Issuer data transmitted to card for online Issuer authentication.
        /// </remarks>
        iss_auth_data = 0x91,

        /// <summary>
        /// Issuer Public Key Remainder
        /// </summary>
        /// <remarks>
        /// Portion of the Issuer Public Key Modulus which does not fit into the Issuer PK Certificate.
        /// </remarks>
        iss_pubkey_rem = 0x92,

        /// <summary>
        /// Signed Static Application Data (SAD)
        /// </summary>
        /// <remarks>
        /// Digital signature on critical application parameters that is used in static data authentication (SDA).
        /// </remarks>
        signed_sad = 0x93,

        /// <summary>
        /// Application File Locator (AFL)
        /// </summary>
        /// <remarks>
        /// Indicates the location (SFI range of records) of the Application Elementary Files associated with a 
        /// particular AID, and read by the Kernel during a transaction.
        /// </remarks>
        app_file_locator = 0x94,

        /// <summary>
        /// Terminal Verification Results (TVR)
        /// </summary>
        /// <remarks>
        /// Status of the different functions from the Terminal perspective. 
        /// The Terminal Verification Results is coded according to Annex C.5 of [EMV Book 3].
        /// For EMV mode transactions, all of the TVR bits sent online to the acquirer shall be set to 0b.
        /// </remarks>
        term_veri_result = 0x95,

        /// <summary>
        /// Transaction Certificate Data Object List (TDOL)
        /// </summary>
        /// <remarks>
        /// List of data objects (tag and length) to be used by the terminal in generating the TC Hash Value
        /// </remarks>
        tdol = 0x97,

        /// <summary>
        /// Transaction Certificate (TC) Hash Value
        /// </summary>
        /// <remarks>
        /// Result of a hash function specified in Book 2, Annex B3.1
        /// </remarks>
        tc_hash = 0x98,

        /// <summary>
        ///Transaction Personal Identification Number (PIN) Data
        /// </summary>
        /// <remarks>
        /// Data entered by the cardholder for the purpose of the PIN verification
        /// </remarks>
        pin_data = 0x99,

        /// <summary>
        /// The tran_date.
        /// </summary>
        /// <remarks>
        /// Local date that the transaction was authorised
        /// </remarks>
        tran_local_date = 0x9a,

        /// <summary>
        /// Transaction Status Information (TSI)
        /// </summary>
        /// <remarks>
        /// Indicates the functions performed in a transaction
        /// </remarks>
        txn_status_info = 0x9b,

        /// <summary>
        /// Transaction Type
        /// </summary>
        /// <remarks>
        /// Indicates the type of financial transaction, represented by the first two digits of 
        /// the ISO 8583:1987 Processing Code. The actual values to be used for the Transaction 
        /// Type data element are defined by the relevant payment system
        /// </remarks>
        tran_type = 0x9c,

        /// <summary>
        /// Directory Definition File (DDF) Name
        /// </summary>
        /// <remarks>
        /// Identifies the name of a DF associated with a directory
        /// </remarks>
        dir_def_file_name = 0x9d,

        /// <summary>
        /// Acquirer Identifier
        /// </summary>
        /// <remarks>
        /// Uniquely identifies the acquirer within each payment system
        /// </remarks>
        acq_id = 0x9f01,

        /// <summary>
        /// Amount, Authorised (Numeric)
        /// </summary>
        /// <remarks>
        /// Authorised amount of the transaction (excluding adjustments).
        /// This amount is expressed with implicit decimal point corresponding to the minor unit of currency 
        /// as defined by [ISO 4217] (for example the six bytes '00 00 00 00 01 23' represent USD 1.23 when 
        /// the currency code is '840'). If the initial transaction amount needs to be replaced with a revised
        /// transaction amount, the Terminal must provide it before the chokepoint.
        /// </remarks>
        amt_auth_num = 0x9f02,

        /// <summary>
        /// Amount, Other (Numeric)	
        /// </summary>
        /// <remarks>
        /// Secondary amount associated with the transaction representing a cashback amount
        /// This amount is expressed with implicit decimal point corresponding to the minor unit of currency as defined by [ISO 4217] 
        /// (for example the 6 bytes '00 00 00 00 01 23' represent GBP 1.23 when the currency code is '826').
        /// </remarks>
        amt_other_num = 0x9f03,

        /// <summary>
        /// Amount, Other (Binary)
        /// </summary>
        /// <remarks>
        /// Secondary amount associated with the transaction representing a cashback amount
        /// </remarks>
        amt_other_bin = 0x9f04,

        /// <summary>
        /// Application Discretionary Data
        /// </summary>
        /// <remarks>
        /// Issuer or payment system specified data relating to the application
        /// </remarks>
        appl_disc_data = 0x9f05,

        /// <summary>
        /// Application Identifier (AID), Terminal
        /// </summary>
        /// <remarks>
        /// Identifies the application as described in ISO/IEC 7816-5
        /// </remarks>
        appl_id = 0x9f06,

        /// <summary>
        /// Application Usage Control (AUC)
        /// </summary>
        /// <remarks>
        /// Indicates issuer's specified restrictions on the geographic usage and services allowed for the application
        /// </remarks>
        appl_usage_cntrl = 0x9f07,

        /// <summary>
        /// Card Application Version Number
        /// </summary>
        /// <remarks>
        /// Version number assigned by the payment system for the application in the Card
        /// </remarks>
        card_app_version_num = 0x9f08,

        /// <summary>
        /// Terminal Application Version Number
        /// </summary>
        /// <remarks>
        /// Version number assigned by the payment system for the Kernel application
        /// </remarks>
        term_app_version_num = 0x9f09,

        /// <summary>
        /// Cardholder Name - Extended
        /// </summary>
        /// <remarks>
        /// Indicates the whole cardholder name when greater than 26 characters using the same coding convention as in ISO 7813
        /// </remarks>
        crdhldrname_ext = 0x9f0b,

        /// <summary>
        /// Issuer Action Code - Default
        /// </summary>
        issuer_action_code_default = 0x9f0d,

        /// <summary>
        /// Issuer Action Code - Denial
        /// </summary>
        issuer_action_code_denial = 0x9f0e,

        /// <summary>
        /// Issuer Action Code - online.
        /// </summary>
        issuer_action_code_online = 0x9f0f,

        /// <summary>
        /// Issuer Application Data (IAD)
        /// </summary>
        /// <remarks>
        /// Contains proprietary application data for transmission to the issuer in an online transaction.
        /// Note: For CCD-compliant applications, Annex C, section C7 defines the specific coding of the 
        /// Issuer Application Data (IAD). To avoid potential conflicts with CCD-compliant applications, 
        /// it is strongly recommended that the IAD data element in an application that is not CCD-compliant
        /// should not use the coding for a CCD-compliant application.
        /// </remarks>
        issuer_app_data = 0x9f10,

        /// <summary>
        /// Issuer Code Table Index
        /// </summary>
        /// <remarks>
        /// Indicates the code table according to ISO/IEC 8859 for displaying the Application Preferred Name
        /// </remarks>
        isssuer_code_tbl = 0x9f11,

        /// <summary>
        /// Application Preferred Name
        /// </summary>
        /// <remarks>
        /// Preferred mnemonic associated with the AID
        /// </remarks>
        card_appl_pre_name = 0x9f12,

        /// <summary>
        /// Last Online Application Transaction Counter (ATC) Register
        /// </summary>
        last_online_atc = 0x9f13,

        /// <summary>
        /// Lower Consecutive Offline Limit (LCOL)
        /// </summary>
        /// <remarks>
        /// Issuer-specified preference for the maximum number of consecutive 
        /// offline transactions for this ICC application allowed in a terminal
        /// with online capability
        /// </remarks>
        lc_offline_lmt = 0x9f14,

        /// <summary>
        /// Merchant Category Code (MCC)
        /// </summary>
        merchant_cat_code = 0x9f15,

        /// <summary>
        /// Merchant Identifier
        /// </summary>
        /// <remarks>
        /// When concatenated with the Acquirer Identifier, uniquely identifies a given merchant
        /// </remarks>
        mer_id = 0x9f16,

        /// <summary>
        /// Personal Identification Number (PIN) Try Counter
        /// </summary>
        /// <remarks>
        /// Number of PIN tries remaining
        /// </remarks>
        pin_try_counter = 0x9f17,

        /// <summary>
        /// Issuer Script Identifier
        /// </summary>
        /// <remarks>
        /// May be sent in authorisation response from issuer when response contains Issuer Script. 
        /// Assigned by the issuer to uniquely identify the Issuer Script.
        /// </remarks>
        issuer_script_id = 0x9f18,

        /// <summary>
        /// Terminal Country Code
        /// </summary>
        /// <remarks>
        /// Indicates the country of the terminal, represented according to ISO 3166-1
        /// </remarks>
        term_county_code = 0x9f1a,

        /// <summary>
        /// Terminal Floor Limit
        /// </summary>
        /// <remarks>
        /// Indicates the floor limit in the terminal in conjunction with the AID
        /// </remarks>
        term_floor_limit = 0x9f1b,

        /// <summary>
        /// Terminal Identification
        /// </summary>
        /// <remarks>
        /// Designates the unique location of a Terminal at a merchant
        /// </remarks>
        temr_id = 0x9f1c,

        /// <summary>
        /// Terminal Risk Management Data
        /// </summary>
        /// <remarks>
        /// Application-specific value used by the card for risk management purposes
        /// </remarks>
        term_riskmgmt_data = 0x9f1d,

        /// <summary>
        /// Interface Device (IFD) Serial Number
        /// </summary>
        /// <remarks>
        /// Unique and permanent serial number assigned to the IFD by the manufacturer
        /// </remarks>
        int_dev_serial_num = 0x9f1e,

        /// <summary>
        /// Track 1 Discretionary Data
        /// </summary>
        /// <remarks>
        /// Discretionary part of track 1 according to ISO/IEC 7813
        /// </remarks>
        track1_disc_data = 0x9f1f,

        /// <summary>
        /// Track 2 Discretionary Data
        /// </summary>
        /// <remarks>
        /// Discretionary part of track 2 according to ISO/IEC 7813
        /// </remarks>
        track2_disc_data = 0x9f20,

        /// <summary>
        /// Transaction Time (HHMMSS)
        /// </summary>
        /// <remarks>
        /// Local time at which the transaction was performed.
        /// </remarks>
        txn_local_time = 0x9f21,

        /// <summary>
        /// Certification Authority Public Key Index (PKI)
        /// </summary>
        /// <remarks>
        /// Identifies the Certificate Authority's public key in conjunction with the RID for use in offline 
        /// static and dynamic data authentication.
        /// </remarks>
        cert_authority_pki = 0x9F22,

        /// <summary>
        /// Upper Consecutive Offline Limit (UCOL)
        /// </summary>
        /// <remarks>
        /// Issuer-specified preference for the maximum number of consecutive offline transactions for this ICC application allowed in a terminal without online capability
        /// </remarks>
        uc_offline_limit = 0x9F23,

        /// <summary>
        /// Payment Account Reference (PAR) generated or linked directly to the provision request in the token vault
        /// </summary>
        /// <remarks>
        /// Payment Account Reference: EMV contact and contactless chip specifications products may support PAR by assigning 
        /// a unique EMV tag (9F24) to represent PAR. PAR SHALL be required personalization data for payment tokens but will 
        /// be optional for terminals to read and transmit.
        /// </remarks>
        payment_account_ref = 0x9F24,

        /// <summary>
        /// Card Application Cryptogram (AC)
        /// </summary>
        /// <remarks>
        /// Cryptogram returned by the ICC in response of the GENERATE AC or RECOVER AC command
        /// </remarks>
        card_app_cryptogram = 0x9f26,

        /// <summary>
        /// Card Cryptogram Information Data (CID)
        /// </summary>
        /// <remarks>
        /// Indicates the type of cryptogram and the actions to be performed by the Kernel. 
        /// The Cryptogram Information Data is coded according to Table 14 of [EMV Book 3].
        /// </remarks>
        card_crypt_info = 0x9f27,

        /// <summary>
        /// Card Extended Selection
        /// </summary>
        /// <remarks>
        /// The value to be appended to the ADF Name in the data field of the SELECT command, 
        /// if the Extended Selection Support flag is present and set to 1. Content is payment
        /// system proprietary.
        /// </remarks>
        card_ext_selection = 0x9F29,

        /// <summary>
        /// Card Kernel Identifier
        /// </summary>
        /// <remarks>
        /// Indicates the card's preference for the kernel on which the contactless application can be processed.
        /// </remarks>
        card_kernal_id = 0x9F2A,

        /// <summary>
        /// Integrated Circuit Card (ICC) PIN Encipherment Public Key Certificate
        /// </summary>
        /// <remarks>
        /// ICC PIN Encipherment Public Key certified by the issuer
        /// </remarks>
        card_icc_pin_cert = 0x9F2D,

        /// <summary>
        /// Integrated Circuit Card (ICC) PIN Encipherment Public Key Exponent
        /// </summary>
        /// <remarks>
        /// ICC PIN Encipherment Public Key Exponent used for PIN encipherment
        /// </remarks>
        card_icc_pin_cert_exponent = 0x9F2E,

        /// <summary>
        /// Issuer Public Key Exponent
        /// </summary>
        /// <remarks>
        /// Issuer public key exponent used for the verification of the Signed Static Application Data and the ICC Public Key Certificate
        /// </remarks>
        card_issue_pk_exponent = 0x9F32,

        /// <summary>
        /// Terminal Capabilities
        /// </summary>
        /// <remarks>
        /// Indicates the card data input, CVM, and security capabilities of the Terminal and Reader. 
        /// The CVM capability (Byte 2) is instantiated with values depending on the transaction amount.
        /// The Terminal Capabilities is coded according to Annex A.2 of [EMV Book 4].
        /// </remarks>
        term_capabilities = 0x9f33,

        /// <summary>
        /// Terminal Cardholder Verification Method (CVM) Results
        /// </summary>
        term_holder_verify_res = 0x9f34,

        /// <summary>
        /// Terminal Type
        /// </summary>
        /// <remarks>
        /// Indicates the environment of the terminal, its communications capability, and its operational control
        /// </remarks>
        term_type = 0x9f35,

        /// <summary>
        /// Card Application Transaction Counter (ATC)
        /// </summary>
        card_app_txn_cnt = 0x9f36,

        /// <summary>
        /// Terminal Unpredictable Number (UN)
        /// </summary>
        /// <remarks>
        /// Contains a Kernel challenge (random) to be used by the Card to ensure the variability and uniqueness 
        /// to the generation of a cryptogram during an EMV mode transaction.
        /// </remarks>
        term_unpred_num = 0x9f37,

        /// <summary>
        /// Processing Options Data Object List (PDOL)
        /// </summary>
        /// <remarks>
        /// List of terminal/reader-related data objects (tags and lengths) requested
        /// by the card to be transmitted in the GET PROCESSING OPTIONS command.
        /// </remarks>
        card_processing_options = 0x9F38,

        /// <summary>
        /// Point-of-Service (POS) Entry Mode
        /// </summary>
        /// <remarks>
        /// Indicates the method by which the PAN was entered, according to the first two digits of the ISO 8583:1987 POS Entry Mode
        /// </remarks>
        term_pos_entry_mode = 0x9F39,

        /// <summary>
        /// Terminal Amount, Reference Currency (Binary)
        /// </summary>
        /// <remarks>
        /// Authorised amount expressed in the reference currency
        /// </remarks>
        term_amt_ref_curency = 0x9F3A,

        /// <summary>
        /// Currency Code, Application Reference
        /// </summary>
        /// <remarks>
        /// 1-4 currency codes used between the terminal and the ICC when the Transaction Currency Code is 
        /// different from the Application Currency Code; each code is 3 digits according to ISO 4217
        /// </remarks>
        card_app_currency_code = 0x9F3B,

        /// <summary>
        /// Terminal Currency Code, Transaction Reference
        /// </summary>
        /// <remarks>
        /// Code defining the common currency used by the terminal in case the Transaction Currency Code 
        /// is different from the Application Currency Code
        /// </remarks>
        term_currency_txn_ref = 0x9F3C,

        /// <summary>
        /// Additional Terminal Capabilities (ATC)
        /// </summary>
        /// <remarks>
        /// Indicates the data input and output capabilities of the Terminal and Reader. 
        /// The Additional Terminal Capabilities is coded according to Annex A.3 of [EMV Book 4].
        /// </remarks>
        term_adtnal_cap = 0x9f40,

        /// <summary>
        /// Terminal Transaction Sequence Counter
        /// </summary>
        /// <remarks>
        /// Counter maintained by the terminal that is incremented by one for each transaction
        /// </remarks>
        term_trans_seq_counter = 0x9f41,


        /// <summary>
        /// Card Currency Code, Application
        /// </summary>
        /// <remarks>
        /// Indicates the currency in which the account is managed according to ISO 4217
        /// </remarks>
        card_currency_code = 0x9F42,

        /// <summary>
        /// Currency Exponent, Application Reference
        /// </summary>
        /// <remarks>
        /// Indicates the implied position of the decimal point from the right of the amount, 
        /// for each of the 1-4 reference currencies represented according to ISO 4217
        /// </remarks>
        card_currency_ref_exponent = 0x9F43,

        /// <summary>
        /// Currency Exponent, Application
        /// </summary>
        /// <remarks>
        /// Indicates the implied position of the decimal point from the right of the amount represented according to ISO 4217
        /// </remarks>
        card_currency_exponent = 0x9F44,

        /// <summary>
        /// Data Authentication Code
        /// </summary>
        /// <remarks>
        /// An issuer assigned value that is retained by the terminal during the verification process of the Signed Static Application Data
        /// </remarks>
        data_auth_code = 0x9f45,

        /// <summary>
        /// Card Dynamic Data Authentication Data Object List (DDOL)
        /// </summary>
        /// <remarks>
        /// List of data objects (tag and length) to be passed to the ICC in the INTERNAL AUTHENTICATE command
        /// </remarks>
        card_dynam_data_list = 0x9F49,

        /// <summary>
        /// Card Static Data Authentication Tag List (SDA)
        /// </summary>
        /// <remarks>
        /// List of tags of primitive data objects defined in this specification whose value fields are to be
        /// included in the Signed Static or Dynamic Application Data
        /// </remarks>
        card_static_data_list = 0x9F4A,

        /// <summary>
        /// Card ICC Dynamic Number
        /// </summary>
        /// <remarks>
        /// Time-variant number generated by the ICC, to be captured by the terminal
        /// </remarks>
        card_icc_dynamic_num = 0x9f4c,

        /// <summary>
        /// Terminal - Merchant Name and Location
        /// </summary>
        /// <remarks>
        /// Indicates the name and location of the merchant. The reader shall return the value of the Merchant 
        /// Name and Location when requested by the card in a Data Object List.
        /// </remarks>
        term_location = 0x9F4E,

        /// <summary>
        /// Offline Accumulator Balance
        /// </summary>
        /// <remarks>
        /// Represents the amount of offline spending available in the Card. 
        /// The Offline Accumulator Balance is retrievable by the GET DATA command, 
        /// if allowed by the Card configuration.
        /// </remarks>
        card_offline_balance_accum = 0x9F50,

        /// <summary>
        /// Transaction Category Code
        /// </summary>
        /// <remarks>
        /// This is a data object defined by MasterCard which indicates the type of transaction being performed, 
        /// and which may be used in card risk management.
        /// </remarks>
        card_txn_cat_code = 0x9F53,

        /// <summary>
        /// Card Cumulative Total Transaction Amount Limit (CTTAL)
        /// </summary>
        card_tot_txn_amt_lim = 0x9F54,

        /// <summary>
        /// Issuer Country Code	
        /// </summary>
        card_issuer_country_code = 0x9F57,

        /// <summary>
        /// Offline Balance
        /// </summary>
        /// <remarks>
        /// In the case of a prepaid card, represents the value stored in card. May be returned in the GENERATE AC response.
        /// </remarks>
        card_offline_balance = 0x9F5F,


        /// <summary>
        /// Card Additional Processes	
        /// </summary>
        card_additional_process = 0x9F68,

        /// <summary>
        /// Card Transaction Qualifiers (CTQ)
        /// </summary>
        /// <remarks>
        /// In this version of the specification, used to indicate to the device the card CVM requirements, 
        /// issuer preferences, and card capabilities.
        /// </remarks>
        card_txn_qualifiers = 0x9F6C,

        /// <summary>
        /// Third Party Data
        /// Visa/Diners: Contactless Form Factor Indicator
        /// Mandatory for Visa and Diners Contactless transactions.
        /// MasterCard: Third Party Data
        /// First 8 characters of the value.
        /// </summary>
        /// <remarks>
        /// The Third Party Data contains various information, possibly including information from a third party. 
        /// If present in the Card, the Third Party Data must be returned in a file read using the READ RECORD command 
        /// or in the File Control Information Template. 'Device Type' is present when the most significant bit of 
        /// byte 1 of 'Unique Identifier' is set to 0b. In this case, the maximum length of 'Proprietary Data' is 26 bytes. 
        /// Otherwise it is 28 bytes.
        /// </remarks>
        third_party_data = 0x9F6E,

        /// <summary>
        /// DS Slot Management Control
        /// </summary>
        /// <remarks>
        /// Contains the Card indication, obtained in the response to the GET PROCESSING OPTIONS command, about the
        /// status of the slot containing data associated to the DS Requested Operator ID.
        /// </remarks>
        card_ds_slot_mgmt_cntrl = 0x9F6F,

        /// <summary>
        /// Customer Exclusive Data
        /// </summary>
        /// <remarks>
        /// Contains data for transmission to the issuer.
        /// </remarks>
        customer_exclusive_data = 0x9F7C,

        /// <summary>
        /// PIN Try Limit
        /// </summary>
        card_pin_try_limit = 0xC6,

        /// <summary>
        /// PIN Try Counter (VSDC Application)
        /// </summary>
        card_pin_try_cnt = 0xDf04,

        /// <summary>
        /// Vehicle Registration Number
        /// </summary>
        card_vehicle_reg_num = 0xDF12,

        /// <summary>
        /// The icc_request.
        /// </summary>
        icc_request = 0xff20,

        /// <summary>
        /// The icc_response.
        /// </summary>
        icc_response = 0xff21
        // ReSharper restore InconsistentNaming
    }
}