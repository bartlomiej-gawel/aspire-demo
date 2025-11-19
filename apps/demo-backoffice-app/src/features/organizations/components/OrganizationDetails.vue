<script setup lang="ts">
import { computed } from 'vue'
import { mockOrganizations } from '../data/mock-organizations'
import { Button } from '@/components/ui/button'
import { ArrowLeft } from 'lucide-vue-next'
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs'
import OrganizationOverviewTab from './OrganizationOverviewTab.vue'
import OrganizationEmployeesTab from './OrganizationEmployeesTab.vue'

interface Props {
  organizationId: string
}

const props = defineProps<Props>()

const organization = computed(() => {
  return mockOrganizations.find(org => org.id === props.organizationId)
})
</script>

<template>
  <div v-if="organization" class="flex flex-1 flex-col gap-4">
    <!-- Header with Back Button -->
    <div>
      <Button variant="ghost" size="sm" as-child class="mb-3">
        <a href="#organizations">
          <ArrowLeft class="mr-2 h-4 w-4" />
          Back to Organizations
        </a>
      </Button>

      <!-- Organization Name -->
      <div class="mb-6">
        <h2 class="text-3xl font-bold tracking-tight">
          {{ organization.name }}
        </h2>
        <p class="text-muted-foreground mt-1">
          Organization details and management
        </p>
      </div>
    </div>

    <!-- Tabs -->
    <Tabs default-value="overview" class="flex-1">
      <TabsList class="grid w-full max-w-md grid-cols-2">
        <TabsTrigger value="overview">
          Overview
        </TabsTrigger>
        <TabsTrigger value="employees">
          Employees
        </TabsTrigger>
      </TabsList>

      <TabsContent value="overview" class="mt-6">
        <OrganizationOverviewTab :organization="organization" />
      </TabsContent>

      <TabsContent value="employees" class="mt-6">
        <OrganizationEmployeesTab :organization="organization" />
      </TabsContent>
    </Tabs>
  </div>

  <div v-else class="flex flex-1 items-center justify-center">
    <div class="text-center space-y-4">
      <h2 class="text-2xl font-bold">Organization not found</h2>
      <p class="text-muted-foreground">The requested organization could not be found.</p>
      <Button as-child>
        <a href="#organizations">
          Back to Organizations
        </a>
      </Button>
    </div>
  </div>
</template>
