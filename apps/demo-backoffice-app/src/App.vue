<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import AppSidebar from '@/components/AppSidebar.vue'
import OrganizationsList from '@/features/organizations/components/OrganizationsList.vue'
import OrganizationEmployeesList from '@/features/organizations/components/OrganizationEmployeesList.vue'
import OrganizationDetails from '@/features/organizations/components/OrganizationDetails.vue'
import { mockOrganizations } from '@/features/organizations/data/mock-organizations'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Separator } from '@/components/ui/separator'
import {
  SidebarInset,
  SidebarProvider,
  SidebarTrigger,
} from '@/components/ui/sidebar'

const currentView = ref<'organizations' | 'employees' | 'details'>('organizations')
const currentOrganizationId = ref<string | null>(null)

const currentOrganization = computed(() => {
  if (!currentOrganizationId.value) return null
  return mockOrganizations.find(org => org.id === currentOrganizationId.value)
})

function updateView() {
  const hash = window.location.hash.slice(1) // Remove #
  if (hash.startsWith('details/')) {
    const orgId = hash.split('/')[1] || null
    currentView.value = 'details'
    currentOrganizationId.value = orgId
  }
  else if (hash.startsWith('employees/')) {
    const orgId = hash.split('/')[1] || null
    currentView.value = 'employees'
    currentOrganizationId.value = orgId
  }
  else {
    currentView.value = 'organizations'
    currentOrganizationId.value = null
  }
}

onMounted(() => {
  updateView()
  window.addEventListener('hashchange', updateView)
})
</script>

<template>
  <SidebarProvider>
    <AppSidebar />
    <SidebarInset>
      <header class="flex h-16 shrink-0 items-center gap-2 border-b px-4">
        <SidebarTrigger class="-ml-1" />
        <Separator orientation="vertical" class="mr-2 h-4" />
        <Breadcrumb>
          <BreadcrumbList>
            <BreadcrumbItem class="hidden md:block">
              <BreadcrumbLink href="#">
                Backoffice
              </BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator class="hidden md:block" />
            <BreadcrumbItem v-if="currentView === 'organizations'">
              <BreadcrumbPage>Organizations</BreadcrumbPage>
            </BreadcrumbItem>
            <template v-else-if="currentView === 'details'">
              <BreadcrumbItem>
                <BreadcrumbLink href="#organizations">
                  Organizations
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator class="hidden md:block" />
              <BreadcrumbItem>
                <BreadcrumbPage>{{ currentOrganization?.name || 'Unknown' }}</BreadcrumbPage>
              </BreadcrumbItem>
            </template>
            <template v-else-if="currentView === 'employees'">
              <BreadcrumbItem>
                <BreadcrumbLink href="#organizations">
                  Organizations
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator class="hidden md:block" />
              <BreadcrumbItem>
                <BreadcrumbLink :href="`#employees/${currentOrganizationId}`">
                  {{ currentOrganization?.name || 'Unknown' }}
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator class="hidden md:block" />
              <BreadcrumbItem>
                <BreadcrumbPage>Employees</BreadcrumbPage>
              </BreadcrumbItem>
            </template>
          </BreadcrumbList>
        </Breadcrumb>
      </header>
      <div class="flex flex-1 flex-col gap-4 p-4">
        <OrganizationsList v-if="currentView === 'organizations'" />
        <OrganizationDetails
          v-else-if="currentView === 'details'"
          :organization-id="currentOrganizationId!"
        />
        <OrganizationEmployeesList
          v-else-if="currentView === 'employees'"
          :organization-id="currentOrganizationId!"
        />
      </div>
    </SidebarInset>
  </SidebarProvider>
</template>
