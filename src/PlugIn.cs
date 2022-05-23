//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Arjan de Bruijn 

using Landis.Core;
using Landis.Library.PnETCohorts;
using System;
using System.Collections.Generic;
using System.Linq;
using Landis.SpatialModeling;


namespace Landis.Extension.Output.PnET
{
    public class PlugIn
        : ExtensionMain
    {
        public static readonly new ExtensionType Type = new ExtensionType("output");
        public static readonly string ExtensionName = "Output-PnET";


        public static ISiteVar<ISiteCohorts> cohorts;
        public static ISiteVar<Landis.Library.Biomass.Pool> woodyDebris;
        public static ISiteVar<Landis.Library.Biomass.Pool> litter;



        private static int tstep;
        public static int TStep
        {
            get
            {
                return tstep;
            }
        }
        InputParameters parameters;
        static ICore modelCore;

        static IEnumerable<ISpecies> selectedspecies;
        static  OutputVariable Biomass;
        static OutputVariable WoodBiomass;
        static OutputVariable AbovegroundBiomass;
        static OutputVariable WoodySenescence;
        static OutputVariable FoliageSenescence;
        static OutputVariable AET;
        static OutputVariable AETAvg;
        static  OutputVariable CohortsPerSpc;
        static  OutputVariable NonWoodyDebris;
        static  OutputVariable WoodyDebris;
        static  OutputVariable AgeDistribution;
        static OutputVariable AnnualPsn;
        static  OutputVariable BelowGroundBiomass;
        static OutputVariable FoliageBiomass;
        static  OutputVariable LAI;
        static  OutputVariable SpeciesEstablishment;
        static OutputVariable EstablishmentProbability;
        static OutputVariable MonthlyNetPsn;
        static OutputVariable MonthlyFolResp;
        static OutputVariable MonthlyGrossPsn;
        static OutputVariable MonthlyMaintResp;
        static OutputVariable MonthlyAverageAlbedo;
        static OutputVariable MonthlyActiveLayerDepth;
        static OutputVariable MonthlyFrostDepth;
        static OutputCSV AverageAlbedoCSV;
        static OutputCSV AverageActiveLayerCSV;
        static OutputCSV AverageFrostDepthCSV;
        static OutputVariable MonthlyAvgSnowPack;
        static OutputVariable MonthlyAvgWater;
        static OutputVariable MonthlyAvgLAI;
        static OutputVariable Water;
        static  OutputVariable SubCanopyPAR;
        static OutputVariable NSC;
        static OutputVariable PET;
        static OutputAggregatedTable overalloutputs;
        static OutputEstablishmentTable establishmentTable;
        static OutputMortalityTable mortalityTable;

        ISiteVar<Landis.Library.Parameters.Species.AuxParm<bool>> SpeciesWasThere;
        ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> LastBiom;

        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName, Type)
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        public static IEnumerable<ISpecies> SelectedSpecies
        {
            get
            {
                return selectedspecies;
            }
        }
        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser parser = new InputParametersParser();
            parameters = Landis.Data.Load<InputParameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------

        public override void Initialize()
        {
            Timestep = parameters.Timestep;
            selectedspecies = parameters.SelectedSpecies;

            tstep = parameters.Timestep;

            cohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.PnETCohorts.ISiteCohorts>("Succession.CohortsPnET");
            woodyDebris = PlugIn.ModelCore.GetSiteVar<Landis.Library.Biomass.Pool>("Succession.WoodyDebris");
            litter = PlugIn.ModelCore.GetSiteVar<Landis.Library.Biomass.Pool>("Succession.Litter");

            if (parameters.CohortsPerSpecies != null)
            {
                CohortsPerSpc = new OutputVariable(parameters.CohortsPerSpecies, "#");
            }
            if (parameters.SpeciesBiom != null)
            {
                Biomass = new OutputVariable(parameters.SpeciesBiom, "g/m2");
                Biomass.output_table_ecoregions = new OutputTableEcoregions(Biomass.MapNameTemplate);
            }
            if (parameters.SpeciesWoodBiom != null)
            {
                WoodBiomass = new OutputVariable(parameters.SpeciesWoodBiom, "g/m2");
                WoodBiomass.output_table_ecoregions = new OutputTableEcoregions(WoodBiomass.MapNameTemplate);
            }
            if (parameters.SpeciesAbovegroundBiom != null)
            {
                AbovegroundBiomass = new OutputVariable(parameters.SpeciesAbovegroundBiom, "g/m2");
                AbovegroundBiomass.output_table_ecoregions = new OutputTableEcoregions(AbovegroundBiomass.MapNameTemplate);
            }
            if (parameters.SpeciesWoodySenescence != null)
            {
                WoodySenescence = new OutputVariable(parameters.SpeciesWoodySenescence, "g/m2");
                WoodySenescence.output_table_ecoregions = new OutputTableEcoregions(WoodySenescence.MapNameTemplate);
            }
            if (parameters.SpeciesFoliageSenescence != null)
            {
                FoliageSenescence = new OutputVariable(parameters.SpeciesFoliageSenescence, "g/m2");
                FoliageSenescence.output_table_ecoregions = new OutputTableEcoregions(FoliageSenescence.MapNameTemplate);
            }
            if (parameters.AET != null)
            {
                AET = new OutputVariable(parameters.AET, "");
                AET.output_table_ecoregions = new OutputTableEcoregions(AET.MapNameTemplate);
            }
            if (parameters.BelowgroundBiom != null)
            {
                BelowGroundBiomass = new OutputVariable(parameters.BelowgroundBiom, "g/m2");
            }
            if (parameters.FoliageBiom != null)
            {
                FoliageBiomass = new OutputVariable(parameters.FoliageBiom, "g/m2");
            }
            if (parameters.LeafAreaIndex != null)
            {
                LAI = new OutputVariable(parameters.LeafAreaIndex, "m2");
                LAI.output_table_ecoregions = new OutputTableEcoregions(LAI.MapNameTemplate);
            }
            if (parameters.MonthlyFolResp != null)
            {
                MonthlyFolResp = new OutputVariable(parameters.MonthlyFolResp, "gC/mo");
            }
            if (parameters.MonthlyGrossPsn != null)
            {
                MonthlyGrossPsn = new OutputVariable(parameters.MonthlyGrossPsn, "gC/mo");
            }
            if (parameters.MonthlyMaintResp != null)
            {
                MonthlyMaintResp = new OutputVariable(parameters.MonthlyMaintResp, "gC/mo");
            }
            if (parameters.MonthlyNetPsn != null)
            {
                MonthlyNetPsn = new OutputVariable(parameters.MonthlyNetPsn, "gC/mo");
            }
            if (parameters.MonthlyAverageAlbedo != null)
            {
                MonthlyAverageAlbedo = new OutputVariable(parameters.MonthlyAverageAlbedo, "ratio_W/m2");
                AverageAlbedoCSV = new OutputCSV(parameters.MonthlyAverageAlbedo.Replace("_{timestep}.img", ".csv"), "ratio_W/m2", "Average Albedo");
            }
            if (parameters.MonthlyActiveLayerDepth != null)
            {
                MonthlyActiveLayerDepth = new OutputVariable(parameters.MonthlyActiveLayerDepth, "cm/mo");
                AverageActiveLayerCSV = new OutputCSV(parameters.MonthlyActiveLayerDepth.Replace("_{timestep}.img", ".csv"), 
                    "cm/mo", "Average Active Layer Depth");
            }
            if (parameters.MonthlyFrostDepth != null)
            {
                MonthlyFrostDepth = new OutputVariable(parameters.MonthlyFrostDepth, "cm/mo");
                AverageFrostDepthCSV = new OutputCSV(parameters.MonthlyFrostDepth.Replace("_{timestep}.img", ".csv"), "cm/mo", "Average Frost Depth");
            }
            if (parameters.MonthlyAvgSnowPack != null)
            {
                MonthlyAvgSnowPack = new OutputVariable(parameters.MonthlyAvgSnowPack, "mm water equivalent");
            }
            if (parameters.MonthlyAvgWater != null)
            {
                MonthlyAvgWater = new OutputVariable(parameters.MonthlyAvgWater, "mm water equivalent");
            }
            if (parameters.MonthlyAvgLAI != null)
            {
                MonthlyAvgLAI = new OutputVariable(parameters.MonthlyAvgLAI, "mm water equivalent");
            }
            if (parameters.EstablishmentProbability != null)
            {
                EstablishmentProbability = new OutputVariable(parameters.EstablishmentProbability, "");

            }
            if (parameters.SpeciesEst != null)
            {
                SpeciesEstablishment = new OutputVariable(parameters.SpeciesEst, "");

            }
            if (parameters.Water != null)
            {
                Water = new OutputVariable(parameters.Water, "mm");
                Water.output_table_ecoregions = new OutputTableEcoregions(Water.MapNameTemplate);
            }

            if (parameters.NSC != null) NSC = new OutputVariable(parameters.NSC, "g/m2");
            if (parameters.PET != null) PET = new OutputVariable(parameters.PET, "mm/mo");
            if (parameters.AETAvg != null) AETAvg = new OutputVariable(parameters.AETAvg, "mm/mo");
            if (parameters.SubCanopyPAR != null) SubCanopyPAR = new OutputVariable(parameters.SubCanopyPAR,  "W/m2 or mmol/m2");
            if (parameters.Litter != null) NonWoodyDebris = new OutputVariable(parameters.Litter, "g/m2");
            if (parameters.WoodyDebris != null) WoodyDebris = new OutputVariable(parameters.WoodyDebris,  "g/m2");
            if (parameters.AgeDistribution != null) AgeDistribution = new OutputVariable(parameters.AgeDistribution, "yr");
            if (parameters.AnnualPsn != null) AnnualPsn = new OutputVariable(parameters.AnnualPsn, "g/m2");
            if (parameters.CohortBalance != null) overalloutputs = new OutputAggregatedTable(parameters.CohortBalance);
            if (parameters.EstablishmentTable != null) establishmentTable = new OutputEstablishmentTable(parameters.EstablishmentTable);
            if (parameters.MortalityTable != null) mortalityTable = new OutputMortalityTable(parameters.MortalityTable);


            MetadataHandler.InitializeMetadata(Timestep, LAI, Biomass, AbovegroundBiomass,WoodBiomass, EstablishmentProbability,
                                               SpeciesWasThere, AnnualPsn, BelowGroundBiomass, FoliageBiomass, CohortsPerSpc, Water, SubCanopyPAR, NonWoodyDebris,
                                               WoodyDebris, AgeDistribution, MonthlyFolResp, MonthlyGrossPsn, MonthlyNetPsn, MonthlyMaintResp,
                                               MonthlyAverageAlbedo, MonthlyActiveLayerDepth, MonthlyFrostDepth, SpeciesEstablishment, NSC, PET, LastBiom, overalloutputs, parameters.CohortBalance);
        }


        public override void Run()
        {

            if (LAI != null)
            {
                System.Console.WriteLine("Updating output variable: LAI");
                // Total LAI per site 

                ISiteVar<float> values = cohorts.GetIsiteVar(o => o.CanopyLAImax);

                string FileName = FileNames.ReplaceTemplateVars(LAI.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<float, float>(FileName, values);

                // Values per species each time step
                LAI.output_table_ecoregions.WriteUpdate(PlugIn.ModelCore.CurrentTime, values);
            }
            if (Biomass != null)
            {
                System.Console.WriteLine("Updating output variable: Biomass");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> Biom = cohorts.GetIsiteVar(o => o.BiomassPerSpecies);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    ISiteVar<int> Biom_spc = modelCore.Landscape.NewSiteVar<int>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        Biom_spc[site] = Biom[site][spc];
                    }

                    new OutputMapSpecies(Biom_spc, spc, Biomass.MapNameTemplate);
                }

                OutputFilePerTStepPerSpecies.Write<int>(Biomass.MapNameTemplate, Biomass.units, PlugIn.ModelCore.CurrentTime, Biom);

                ISiteVar<float> Biomass_site = cohorts.GetIsiteVar(x => x.BiomassSum);
                string FileName = FileNames.ReplaceTemplateVars(Biomass.MapNameTemplate, "AllSpecies", PlugIn.ModelCore.CurrentTime);
                new OutputMapSiteVar<float, float>(FileName, Biomass_site, o => o);
                Biomass.output_table_ecoregions.WriteUpdate<float>(PlugIn.ModelCore.CurrentTime, Biomass_site);
            }
            if (AbovegroundBiomass != null)
            {
                System.Console.WriteLine("Updating output variable: Aboveground Biomass");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> AGBiom = cohorts.GetIsiteVar(o => o.AbovegroundBiomassPerSpecies);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    ISiteVar<int> AGBiom_spc = modelCore.Landscape.NewSiteVar<int>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        AGBiom_spc[site] = AGBiom[site][spc];
                    }

                    new OutputMapSpecies(AGBiom_spc, spc, AbovegroundBiomass.MapNameTemplate);
                }

                OutputFilePerTStepPerSpecies.Write<int>(AbovegroundBiomass.MapNameTemplate, AbovegroundBiomass.units, PlugIn.ModelCore.CurrentTime, AGBiom);

                ISiteVar<float> AGBiomass_site = cohorts.GetIsiteVar(x => x.AbovegroundBiomassSum);
                string FileName = FileNames.ReplaceTemplateVars(AbovegroundBiomass.MapNameTemplate, "AllSpecies", PlugIn.ModelCore.CurrentTime);
                new OutputMapSiteVar<float, float>(FileName, AGBiomass_site, o => o);
                AbovegroundBiomass.output_table_ecoregions.WriteUpdate<float>(PlugIn.ModelCore.CurrentTime, AGBiomass_site);
            }
            if (WoodBiomass != null)
            {
                System.Console.WriteLine("Updating output variable: WoodBiomass");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> WoodBiom = cohorts.GetIsiteVar(o => o.WoodBiomassPerSpecies);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    ISiteVar<int> WoodBiom_spc = modelCore.Landscape.NewSiteVar<int>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        WoodBiom_spc[site] = WoodBiom[site][spc];
                    }

                    new OutputMapSpecies(WoodBiom_spc, spc, WoodBiomass.MapNameTemplate);
                }

                OutputFilePerTStepPerSpecies.Write<int>(WoodBiomass.MapNameTemplate, AbovegroundBiomass.units, PlugIn.ModelCore.CurrentTime, WoodBiom);

                ISiteVar<float> WoodBiomass_site = cohorts.GetIsiteVar(x => x.WoodBiomassSum);
                string FileName = FileNames.ReplaceTemplateVars(WoodBiomass.MapNameTemplate, "AllSpecies", PlugIn.ModelCore.CurrentTime);
                new OutputMapSiteVar<float, float>(FileName, WoodBiomass_site, o => o);
                WoodBiomass.output_table_ecoregions.WriteUpdate<float>(PlugIn.ModelCore.CurrentTime, WoodBiomass_site);
            }
            if (BelowGroundBiomass != null)
            {
                System.Console.WriteLine("Updating output variable: BelowGroundBiomass");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> RootBiom = cohorts.GetIsiteVar(o => o.BelowGroundBiomassPerSpecies);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    ISiteVar<int> RootBiom_spc = modelCore.Landscape.NewSiteVar<int>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        RootBiom_spc[site] = RootBiom[site][spc];
                    }

                    new OutputMapSpecies(RootBiom_spc, spc, BelowGroundBiomass.MapNameTemplate);
                }

                OutputFilePerTStepPerSpecies.Write<int>(BelowGroundBiomass.MapNameTemplate, BelowGroundBiomass.units, PlugIn.ModelCore.CurrentTime, RootBiom);

                ISiteVar<float> values = cohorts.GetIsiteVar(o => o.BelowGroundBiomassSum);
                string FileName = FileNames.ReplaceTemplateVars(BelowGroundBiomass.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);
                new OutputMapSiteVar<float, float>(FileName, values, o => o);
            }
            if (FoliageBiomass != null)
            {
                System.Console.WriteLine("Updating output variable: FoliageBiomass");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> FolBiom = cohorts.GetIsiteVar(o => o.FoliageBiomassPerSpecies);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    ISiteVar<int> FolBiom_spc = modelCore.Landscape.NewSiteVar<int>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        FolBiom_spc[site] = FolBiom[site][spc];
                    }

                    new OutputMapSpecies(FolBiom_spc, spc, FoliageBiomass.MapNameTemplate);
                }

                OutputFilePerTStepPerSpecies.Write<int>(FoliageBiomass.MapNameTemplate, FoliageBiomass.units, PlugIn.ModelCore.CurrentTime, FolBiom);

                ISiteVar<float> values = cohorts.GetIsiteVar(o => o.FoliageSum);
                string FileName = FileNames.ReplaceTemplateVars(FoliageBiomass.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);
                new OutputMapSiteVar<float, float>(FileName, values, o => o);
            }
            if (NSC != null)
            {
                System.Console.WriteLine("Updating output variable: NSC");

                ISiteVar<float> values = cohorts.GetIsiteVar(o => o.NSCSum);

                string FileName = FileNames.ReplaceTemplateVars(NSC.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<float, float>(FileName, values, o => o);
            }
            if (PET != null)
            {
                System.Console.WriteLine("Updating output variable: PET");

                ISiteVar<float> values = cohorts.GetIsiteVar(o => o.PET);

                string FileName = FileNames.ReplaceTemplateVars(PET.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<float, float>(FileName, values, o => o);
            }
            if (WoodySenescence != null)
            {
                System.Console.WriteLine("Updating output variable: Woody Senescence");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> Senes = cohorts.GetIsiteVar(o => o.WoodySenescencePerSpecies);

                OutputFilePerTStepPerSpecies.Write<int>(WoodySenescence.MapNameTemplate, WoodySenescence.units, PlugIn.ModelCore.CurrentTime, Senes);

                ISiteVar<float> Senescence_site = cohorts.GetIsiteVar(x => x.WoodySenescenceSum);

                WoodySenescence.output_table_ecoregions.WriteUpdate<float>(PlugIn.ModelCore.CurrentTime, Senescence_site);
            }
            if (FoliageSenescence != null)
            {
                System.Console.WriteLine("Updating output variable: Foliage Senescence");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> Senes = cohorts.GetIsiteVar(o => o.FoliageSenescencePerSpecies);

                OutputFilePerTStepPerSpecies.Write<int>(FoliageSenescence.MapNameTemplate, FoliageSenescence.units, PlugIn.ModelCore.CurrentTime, Senes);

                ISiteVar<float> Senescence_site = cohorts.GetIsiteVar(x => x.FoliageSenescenceSum);

                FoliageSenescence.output_table_ecoregions.WriteUpdate<float>(PlugIn.ModelCore.CurrentTime, Senescence_site);
            }
            if (AET != null)
            {
                ISiteVar<float> AET_site = cohorts.GetIsiteVar(x => x.AETSum);

                AET.output_table_ecoregions.WriteUpdate<float>(PlugIn.ModelCore.CurrentTime, AET_site);
            }
            if (AETAvg != null)
            {
                System.Console.WriteLine("Updating output variable: AETAvg");

                ISiteVar<float> values = cohorts.GetIsiteVar(o => o.AETSum / (float)PlugIn.modelCore?.Landscape?.SiteCount);

                string FileName = FileNames.ReplaceTemplateVars(AETAvg.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<float, float>(FileName, values, o => o);
            }
            if (MonthlyFolResp != null)
            {
                ISiteVar<float[]> monthlyFolResp = cohorts.GetIsiteVar(site => site.FolResp);

                WriteMonthlyOutput(monthlyFolResp, MonthlyFolResp.MapNameTemplate);
            }
            if (MonthlyGrossPsn != null)
            {
                ISiteVar<float[]> monthlyGrossPsn = cohorts.GetIsiteVar(site => site.GrossPsn);

                WriteMonthlyOutput(monthlyGrossPsn, MonthlyGrossPsn.MapNameTemplate);
            }
            if (MonthlyNetPsn != null)
            {
                ISiteVar<float[]> monthlyNetPsn = cohorts.GetIsiteVar(site => site.NetPsn);

                WriteMonthlyOutput(monthlyNetPsn, MonthlyNetPsn.MapNameTemplate);
            }
            if (MonthlyMaintResp != null)
            {
                ISiteVar<float[]> monthlyMaintResp = cohorts.GetIsiteVar(site => site.MaintResp);

                WriteMonthlyOutput(monthlyMaintResp, MonthlyMaintResp.MapNameTemplate);
            }
            if (MonthlyAverageAlbedo != null)
            {
                System.Console.WriteLine("Updating output variable: Monthly Average Albedo");
                ISiteVar<float[]> monthlyAverageAlbedo = cohorts.GetIsiteVar(site => site.AverageAlbedo);

                WriteMonthlyDecimalOutput(monthlyAverageAlbedo, MonthlyAverageAlbedo.MapNameTemplate, 100);
                AverageAlbedoCSV.WriteAverageFromMonthly(PlugIn.ModelCore.CurrentTime, monthlyAverageAlbedo);
            }
            if (MonthlyActiveLayerDepth != null && PlugIn.ModelCore.CurrentTime != 0)
            {
                // Only interested in summer months for this output
                bool[] monthsToOutput = new bool[] { false, false, false, false, true, true, true, true, false, false, false, false};

                System.Console.WriteLine("Updating output variable: Monthly Active Layer Depth");
                ISiteVar<float[]> monthlyActiveLayerDepth = cohorts.GetIsiteVar(site => site.ActiveLayerDepth);

                WriteMonthlyDecimalOutput(monthlyActiveLayerDepth, MonthlyActiveLayerDepth.MapNameTemplate, 100, -1, 
                    monthsToOutput);
                AverageActiveLayerCSV.WriteAverageFromMonthly(PlugIn.ModelCore.CurrentTime, monthlyActiveLayerDepth);
            }
            if (MonthlyFrostDepth != null && PlugIn.ModelCore.CurrentTime != 0)
            {
                // Only interested in winter months for this output
                bool[] monthsToOutput = new bool[] { true, true, true, false, false, false, false, false, false, false, false, true };

                System.Console.WriteLine("Updating output variable: Monthly Frost Depth");
                ISiteVar<float[]> monthlyFrostDepth = cohorts.GetIsiteVar(site => site.FrostDepth);

                WriteMonthlyDecimalOutput(monthlyFrostDepth, MonthlyFrostDepth.MapNameTemplate, 100, -1,
                    monthsToOutput);
                AverageFrostDepthCSV.WriteAverageFromMonthly(PlugIn.ModelCore.CurrentTime, monthlyFrostDepth);
            }
            if(MonthlyAvgSnowPack != null && PlugIn.ModelCore.CurrentTime != 0)
            {
                System.Console.WriteLine("Updating output variable: Monthly Avg Snow Pack");
                ISiteVar<float[]> monthlyAvgSnowPack = cohorts.GetIsiteVar(site => site.MonthlyAvgSnowPack);

                WriteMonthlyOutput(monthlyAvgSnowPack, MonthlyAvgSnowPack.MapNameTemplate);
            }
            if (MonthlyAvgWater != null && PlugIn.ModelCore.CurrentTime != 0)
            {
                System.Console.WriteLine("Updating output variable: Monthly Avg Water");
                ISiteVar<float[]> monthlyAvgWater = cohorts.GetIsiteVar(site => site.MonthlyAvgWater);

                WriteMonthlyOutput(monthlyAvgWater, MonthlyAvgWater.MapNameTemplate);
            }
            if (MonthlyAvgLAI != null && PlugIn.ModelCore.CurrentTime != 0)
            {
                System.Console.WriteLine("Updating output variable: Monthly Avg LAI");
                ISiteVar<float[]> monthlyAvgLAI = cohorts.GetIsiteVar(site => site.MonthlyAvgLAI);

                WriteMonthlyOutput(monthlyAvgLAI, MonthlyAvgLAI.MapNameTemplate);
            }
            if (CohortsPerSpc != null)
            {
                System.Console.WriteLine("Updating output variable: CohortsPerSpc");
                // Nr of Cohorts per site and per species

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<int>> cps =  cohorts.GetIsiteVar(x => x.CohortCountPerSpecies);

                new OutputHistogramCohort<int>(CohortsPerSpc.MapNameTemplate, "CohortsPerSpcPerSite", 10).WriteOutputHist(cps);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    string FileName = FileNames.ReplaceTemplateVars(CohortsPerSpc.MapNameTemplate, spc.Name, PlugIn.ModelCore.CurrentTime);

                    new OutputMapSiteVar<Landis.Library.Parameters.Species.AuxParm<int>, int>(FileName, cps, o => o[spc]);
                }

                OutputFilePerTStepPerSpecies.WriteSum<int>(CohortsPerSpc.MapNameTemplate, CohortsPerSpc.units, PlugIn.ModelCore.CurrentTime, cps);
            }
            if (EstablishmentProbability != null)
            {
                System.Console.WriteLine("Updating output variable: EstablishmentProbability");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<byte>> pest = (ISiteVar<Landis.Library.Parameters.Species.AuxParm<byte>>)cohorts.GetIsiteVar(o => o.EstablishmentProbability.Probability);

                foreach (ISpecies spc in PlugIn.SelectedSpecies)
                {
                    ISiteVar<int> _pest = modelCore.Landscape.NewSiteVar<int>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        _pest[site] = pest[site][spc];
                    }

                    new OutputMapSpecies(_pest, spc, EstablishmentProbability.MapNameTemplate);
                }

            }
            if (SpeciesEstablishment != null)
            {
                System.Console.WriteLine("Updating output variable: SpeciesEstablishment");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<bool>> SpeciesIsThere = cohorts.GetIsiteVar(o => o.SpeciesPresent);

                if (SpeciesWasThere != null)
                {
                    foreach (ISpecies spc in PlugIn.modelCore.Species)
                    {
                        ISiteVar<int> comp = PlugIn.modelCore.Landscape.NewSiteVar<int>();

                        MapComparison m = new MapComparison();
                        foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                        {
                            if (SpeciesWasThere[site] == null)
                            {
                                SpeciesWasThere[site] = new Library.Parameters.Species.AuxParm<bool>(PlugIn.modelCore.Species);
                            }

                            comp[site] = m[SpeciesWasThere[site][spc], SpeciesIsThere[site][spc]];

                            SpeciesWasThere[site][spc] = SpeciesIsThere[site][spc];
                        }


                        OutputMapSpecies output_map =  new OutputMapSpecies(comp, spc, SpeciesEstablishment.MapNameTemplate);

                        // map label text
                        //m.PrintLabels(SpeciesEstablishment.MapNameTemplate, spc);


                    }
                }
                else
                {
                    SpeciesWasThere = modelCore.Landscape.NewSiteVar<Landis.Library.Parameters.Species.AuxParm<bool>>();

                    foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                    {
                        SpeciesWasThere[site] = new Library.Parameters.Species.AuxParm<bool>(PlugIn.modelCore.Species);
                    }
                }

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<bool>> Established_spc = cohorts.GetIsiteVar(x => x.SpeciesPresent);

                Landis.Library.Parameters.Species.AuxParm<int> Est_Sum = new Landis.Library.Parameters.Species.AuxParm<int>(PlugIn.modelCore.Species);

                foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
                    {
                    foreach (ISpecies spc in PlugIn.modelCore.Species)
                        {
                        if (Established_spc[site][spc] == true)
                        {
                            Est_Sum[spc]++;
                        }
                    }
                }

                OutputFilePerTStepPerSpecies.Write<int>(SpeciesEstablishment.MapNameTemplate, SpeciesEstablishment.units, PlugIn.ModelCore.CurrentTime, Est_Sum);

            }
            if (AnnualPsn != null)
            {
                System.Console.WriteLine("Updating output variable: AnnualPsn");

                ISiteVar<int> annualNetPsn = cohorts.GetIsiteVar(site => (int)site.NetPsnSum);
                string FileName = FileNames.ReplaceTemplateVars(AnnualPsn.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);
                new OutputMapSiteVar<int, int>(FileName, annualNetPsn, o => o);
            }

            if (Water != null)
            {
                System.Console.WriteLine("Updating output variable: Water");

                ISiteVar<float> Water_site = cohorts.GetIsiteVar(x => x.WaterAvg * 100);

                string FileName = FileNames.ReplaceTemplateVars(Water.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<float, float>(FileName, Water_site, o => o);

                Water.output_table_ecoregions.WriteUpdate(PlugIn.ModelCore.CurrentTime, Water_site);
            }

            if (SubCanopyPAR != null)
            {
                System.Console.WriteLine("Updating output variable: SubCanopyPAR");

                ISiteVar<float> SubCanopyRadiation = cohorts.GetIsiteVar(x => x.SubCanopyParMAX);

                string FileName = FileNames.ReplaceTemplateVars(SubCanopyPAR.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<float, float>(FileName, SubCanopyRadiation, o => o);


            }
            if (NonWoodyDebris != null)
            {
                System.Console.WriteLine("Updating output variable: NonWoodyDebris");

                ISiteVar<double> Litter = cohorts.GetIsiteVar(x => x.Litter);

                string FileName = FileNames.ReplaceTemplateVars(NonWoodyDebris.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<double, double>(FileName, Litter, o => o);

            }

            if (WoodyDebris != null)
            {
                System.Console.WriteLine("Updating output variable: WoodyDebris");

                ISiteVar<double> woody_debris = cohorts.GetIsiteVar(x => x.WoodyDebris);

                string FileName = FileNames.ReplaceTemplateVars(WoodyDebris.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<double, double>(FileName, woody_debris, o => o);

            }

            if (AgeDistribution != null)
            {
                System.Console.WriteLine("Updating output variable: AgeDistribution");

                ISiteVar<Landis.Library.Parameters.Species.AuxParm<List<ushort>>> values = cohorts.GetIsiteVar(o => o.CohortAges);

                new OutputHistogramCohort<ushort>(AgeDistribution.MapNameTemplate, "NrOfCohortsAtAge", 10).WriteOutputHist(values);


                System.Console.WriteLine("Updating output variable: MaxAges");

                ISiteVar<int> maxage = cohorts.GetIsiteVar(x => x.AgeMax);

                string FileName = FileNames.ReplaceTemplateVars(AgeDistribution.MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                new OutputMapSiteVar<int, int>(FileName, maxage, o => o);

            }
            if (overalloutputs != null)
            {
                System.Console.WriteLine("Updating output variable: overalloutputs");
                OutputAggregatedTable.WriteNrOfCohortsBalance();
            }
            if (establishmentTable != null)
            {
                System.Console.WriteLine("Updating output variable: establishmentTable");
                OutputEstablishmentTable.WriteEstablishmentTable();
        }
            if (mortalityTable != null)
            {
                System.Console.WriteLine("Updating output variable: MortalityTable");
                OutputMortalityTable.WriteMortalityTable();
            }
        }

        private static void WriteMonthlyOutput(ISiteVar<float[]> monthly, string MapNameTemplate, double multiplier = 1, int inactiveValue = 0, bool[] monthsToOutput = null)
        {
            // The default is to output all months
            monthsToOutput = monthsToOutput ?? new bool[] { true, true, true, true, true, true, true, true, true, true, true, true};

            string[] months = new string[] { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };

            for (int mo = 0; mo < months.Count(); mo++)
            {
                // Do not report months that are not included
                if (!monthsToOutput[mo])
                {
                    continue;
                }
                ISiteVar<int> monthlyValue = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

                foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                {
                    monthlyValue[site] = (int)monthly[site][mo];
                }

                string FileName = FileNames.ReplaceTemplateVars(MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                FileName = System.IO.Path.ChangeExtension(FileName, null) + months[mo] + System.IO.Path.GetExtension(FileName);

                new OutputMapSiteVar<int, int>(FileName, monthlyValue, o => o, multiplier, inactiveValue);
            }
        }

        private static void WriteMonthlyDecimalOutput(ISiteVar<float[]> monthly, string MapNameTemplate, double multiplier = 1, float inactiveValue = 0, bool[] monthsToOutput = null)
        {
            // The default is to output all months
            monthsToOutput = monthsToOutput ?? new bool[] { true, true, true, true, true, true, true, true, true, true, true, true };

            string[] months = new string[] { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };

            for (int mo = 0; mo < months.Count(); mo++)
            {
                // Do not report months that are not included
                if (!monthsToOutput[mo])
                {
                    continue;
                }
                ISiteVar<float> monthlyValue = PlugIn.ModelCore.Landscape.NewSiteVar<float>();

                foreach (ActiveSite site in PlugIn.modelCore.Landscape)
                {
                    monthlyValue[site] = monthly[site][mo];
                }

                string FileName = FileNames.ReplaceTemplateVars(MapNameTemplate, "", PlugIn.ModelCore.CurrentTime);

                FileName = System.IO.Path.ChangeExtension(FileName, null) + months[mo] + System.IO.Path.GetExtension(FileName);

                new OutputMapSiteVar<float, float>(FileName, monthlyValue, multiplier, inactiveValue);
            }
        }

    }

}

