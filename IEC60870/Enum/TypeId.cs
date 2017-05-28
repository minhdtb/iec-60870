namespace IEC60870.Enum
{
    public enum TypeId
    {
        /**
         * 1 - Single-point information without time tag
         */
        [Description("Single-point information without time tag")] M_SP_NA_1 = 1,

        /**
         * 2 - Single-point information with time tag
         */
        [Description("Single-point information with time tag")] M_SP_TA_1,

        /**
         * 3 - Double-point information without time tag
         */
        [Description("Double-point information without time tag")] M_DP_NA_1,

        /**
         * 4 - Double-point information with time tag
         */
        [Description("Double-point information with time tag")] M_DP_TA_1,

        /**
         * 5 - Step position information
         */
        [Description("Step position information")] M_ST_NA_1,

        /**
         * 6 - Step position information with time tag
         */
        [Description("Step position information with time tag")] M_ST_TA_1,

        /**
         * 7 - Bitstring of 32 bit
         */
        [Description("Bitstring of 32 bit")] M_BO_NA_1,

        /**
         * 8 - Bitstring of 32 bit with time tag
         */
        [Description("Bitstring of 32 bit with time tag")] M_BO_TA_1,

        /**
         * 9 - Measured value, normalized value
         */
        [Description("Measured value, normalized value")] M_ME_NA_1,

        /**
         * 10 - Measured value, normalized value with time tag
         */
        [Description("Measured value, normalized value with time tag")] M_ME_TA_1,

        /**
         * 11 - Measured value, scaled value
         */
        [Description("Measured value, scaled value")] M_ME_NB_1,

        /**
         * 12 - Measured value, scaled value with time tag
         */
        [Description("Measured value, scaled value with time tag")] M_ME_TB_1,

        /**
         * 13 - Measured value, short floating point number
         */
        [Description("Measured value, short floating point number")] M_ME_NC_1,

        /**
         * 14 - Measured value, short floating point number with time tag
         */
        [Description("Measured value, short floating point number with time tag")] M_ME_TC_1,

        /**
         * 15 - Integrated totals
         */
        [Description("Integrated totals")] M_IT_NA_1,

        /**
         * 16 - Integrated totals with time tag
         */
        [Description("Integrated totals with time tag")] M_IT_TA_1,

        /**
         * 17 - Event of protection equipment with time tag
         */
        [Description("Event of protection equipment with time tag")] M_EP_TA_1,

        /**
         * 18 - Packed start events of protection equipment with time tag
         */
        [Description("Packed start events of protection equipment with time tag")] M_EP_TB_1,

        /**
         * 19 - Packed output circuit information of protection equipment with time tag
         */
        [Description("Packed output circuit information of protection equipment with time tag")] M_EP_TC_1,

        /**
         * 20 - Packed single-point information with status change detection
         */
        [Description("Packed single-point information with status change detection")] M_PS_NA_1,

        /**
         * 21 - Measured value, normalized value without quality descriptor
         */
        [Description("Measured value, normalized value without quality descriptor")] M_ME_ND_1,

        /**
         * 30 - Single-point information with time tag CP56Time2a
         */
        [Description("Single-point information with time tag CP56Time2a")] M_SP_TB_1 = 30,

        /**
         * 31 - Double-point information with time tag CP56Time2a
         */
        [Description("Double-point information with time tag CP56Time2a")] M_DP_TB_1,

        /**
         * 32 - Step position information with time tag CP56Time2a
         */
        [Description("Step position information with time tag CP56Time2a")] M_ST_TB_1,

        /**
         * 33 - Bitstring of 32 bits with time tag CP56Time2a
         */
        [Description("Bitstring of 32 bits with time tag CP56Time2a")] M_BO_TB_1,

        /**
         * 34 - Measured value, normalized value with time tag CP56Time2a
         */
        [Description("Measured value, normalized value with time tag CP56Time2a")] M_ME_TD_1,

        /**
         * 35 - Measured value, scaled value with time tag CP56Time2a
         */
        [Description("Measured value, scaled value with time tag CP56Time2a")] M_ME_TE_1,

        /**
         * 36 - Measured value, short floating point number with time tag CP56Time2a
         */
        [Description("Measured value, short floating point number with time tag CP56Time2a")] M_ME_TF_1,

        /**
         * 37 - Integrated totals with time tag CP56Time2a
         */
        [Description("Integrated totals with time tag CP56Time2a")] M_IT_TB_1,

        /**
         * 38 - Event of protection equipment with time tag CP56Time2a
         */
        [Description("Event of protection equipment with time tag CP56Time2a")] M_EP_TD_1,

        /**
         * 39 - Packed start events of protection equipment with time tag CP56Time2a
         */
        [Description("Packed start events of protection equipment with time tag CP56Time2a")] M_EP_TE_1,

        /**
         * 40 - Packed output circuit information of protection equipment with time tag CP56Time2a
         */
        [Description("Packed output circuit information of protection equipment with time tag CP56Time2a")] M_EP_TF_1,

        /**
         * 45 - Single command
         */
        [Description("Single command")] C_SC_NA_1 = 45,

        /**
         * 46 - Double command
         */
        [Description("Double command")] C_DC_NA_1,

        /**
         * 47 - Regulating step command
         */
        [Description(" Regulating step command")] C_RC_NA_1,

        /**
         * 48 - Set point command, normalized value
         */
        [Description("Set point command, normalized value")] C_SE_NA_1,

        /**
         * 49 - Set point command, scaled value
         */
        [Description("Set point command, scaled value")] C_SE_NB_1,

        /**
         * 50 - Set point command, short floating point number
         */
        [Description("Set point command, short floating point number")] C_SE_NC_1,

        /**
         * 51 - Bitstring of 32 bits
         */
        [Description("Bitstring of 32 bits")] C_BO_NA_1,

        /**
         * 58 - Single command with time tag CP56Time2a
         */
        [Description("Single command with time tag CP56Time2a")] C_SC_TA_1 = 58,

        /**
         * 59 - Double command with time tag CP56Time2a
         */
        [Description("Double command with time tag CP56Time2a")] C_DC_TA_1,

        /**
         * 60 - Regulating step command with time tag CP56Time2a
         */
        [Description("Regulating step command with time tag CP56Time2a")] C_RC_TA_1,

        /**
         * 61 - Set-point command with time tag CP56Time2a, normalized value
         */
        [Description("Set-point command with time tag CP56Time2a, normalized value")] C_SE_TA_1,

        /**
         * 62 - Set-point command with time tag CP56Time2a, scaled value
         */
        [Description("Set-point command with time tag CP56Time2a, scaled value")] C_SE_TB_1,

        /**
         * 63 - C_SE_TC_1 Set-point command with time tag CP56Time2a, short floating point number
         */
        [Description("C_SE_TC_1 Set-point command with time tag CP56Time2a, short floating point number")] C_SE_TC_1,

        /**
         * 64 - Bitstring of 32 bit with time tag CP56Time2a
         */
        [Description("Bitstring of 32 bit with time tag CP56Time2a")] C_BO_TA_1,

        /**
         * 70 - End of initialization
         */
        [Description("End of initialization")] M_EI_NA_1 = 70,

        /**
         * 100 - Interrogation command
         */
        [Description("Interrogation command")] C_IC_NA_1 = 100,

        /**
         * 101 - Counter interrogation command
         */
        [Description("Counter interrogation command")] C_CI_NA_1,

        /**
         * 102 - Read command
         */
        [Description("Read command")] C_RD_NA_1,

        /**
         * 103 - Clock synchronization command
         */
        [Description("Clock synchronization command")] C_CS_NA_1,

        /**
         * 104 - Test command
         */
        [Description("Test command")] C_TS_NA_1,

        /**
         * 105 - Reset process command
         */
        [Description("Reset process command")] C_RP_NA_1,

        /**
         * 106 - Delay acquisition command
         */
        [Description("Delay acquisition command")] C_CD_NA_1,

        /**
         * 107 - Test command with time tag CP56Time2a
         */
        [Description("Test command with time tag CP56Time2a")] C_TS_TA_1,

        /**
         * 110 - Parameter of measured value, normalized value
         */
        [Description("Parameter of measured value, normalized value")] P_ME_NA_1 = 110,

        /**
         * 111 - Parameter of measured value, scaled value
         */
        [Description("Parameter of measured value, scaled value")] P_ME_NB_1,

        /**
         * 112 - Parameter of measured value, short floating point number
         */
        [Description("Parameter of measured value, short floating point number")] P_ME_NC_1,

        /**
         * 113 - Parameter activation
         */
        [Description("Parameter activation")] P_AC_NA_1,

        /**
         * 120 - File ready
         */
        [Description("File ready")] F_FR_NA_1 = 120,

        /**
         * 121 - Section ready
         */
        [Description("Section ready")] F_SR_NA_1,

        /**
         * 122 - Call directory, select file, call file, call section
         */
        [Description("Call directory, select file, call file, call section")] F_SC_NA_1,

        /**
         * 123 - Last section, last segment
         */
        [Description("Last section, last segment")] F_LS_NA_1,

        /**
         * 124 - Ack file, ack section
         */
        [Description("Ack file, ack section")] F_AF_NA_1,

        /**
         * 125 - Segment
         */
        [Description("Segment")] F_SG_NA_1,

        /**
         * 126 - Directory
         */
        [Description("Directory")] F_DR_TA_1,

        /**
         * 127 - QueryLog, request archive file
         */
        [Description("QueryLog, request archive file")] F_SC_NB_1
    }
}